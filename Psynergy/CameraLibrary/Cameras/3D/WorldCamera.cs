using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Psynergy.Camera
{
    public class WorldCamera : FreeCamera, IFocusable
    {
        #region Factory Property setting
        protected override void ClassProperties(Factory factory)
        {
            factory.RegisterVector3("viewpos", "ViewPos");
            factory.RegisterQuat("desiredrot", "DesiredRotation");

            base.ClassProperties(factory);
        }
        #endregion

        private IFocusable3D m_PreviousCameraFocus = null;      // used to go back to the previous camras focus 
        private Vector3 m_PreviousCameraTargetPosition = Vector3.Zero;
        private float m_PreviousCameraDistance = 1.0f;
        
        private Vector3 m_PositionToViewFrom = Vector3.Zero;
        private Quaternion m_DesiredRotation = Quaternion.Identity;
        private float m_RotateAmount = 0.0f;

        private Vector3 m_UnfocusedTarget = Vector3.Zero;

        public WorldCamera() : base("")
        {
        }

        public WorldCamera(String name) : base(name)
        {
        }

        public WorldCamera(String name, Vector3 viewPos, Quaternion startRotation) : base(name, viewPos, startRotation)
        {
            m_PositionToViewFrom = viewPos;
            m_DesiredRotation = Quaternion.CreateFromYawPitchRoll(startRotation.Y, startRotation.X, startRotation.Z);
            m_StartTween = true;
        }

        public override void Initialise()
        {
            base.Initialise();

            // Camera Type
            m_CameraType = CameraType.eCameraType_World;

            // Set world camera speed
            MoveSpeed = 5.0f;
        }

        public override void Load()
        {
            base.Load();
        }

        protected override bool IsExitFinished()
        {
            bool toRet = false;

            if (DistanceToTarget <= 1.0f)
                toRet = true;

            return toRet;
        }

        protected override void OnExitFinished()
        {
            base.OnExitFinished();

            // Change to the third person camera
            CameraManager.Instance.ChangeCamera("FixedThirdPerson");

            // Set the focus to the third person camera if one exists here
            // Remove any focus links if they exist
            if (Focus != null)
            {
                BaseCamera activeCamera = CameraManager.Instance.ActiveCamera;

                if (activeCamera != null)
                {
                    IFocusable3D currentFocus = Focus;

                    // Remove focus
                    Focus = null;
                    
                    // Set the focus onto the new camera
                    activeCamera.SetInstantFocus(currentFocus);
                }
            }
        }

        protected override void UpdateEnter(GameTime deltaTime)
        {
            base.UpdateEnter(deltaTime);
        }

        protected override void OnActive()
        {
            m_CameraState = CameraState.Active;
        }

        protected override void UpdateActive(GameTime deltaTime)
        {
            base.UpdateActive(deltaTime);

            if ((Focus != null) && (!Tween))
            {
                // Previous focus changes now because a new one was set
                m_PreviousCameraFocus = Focus;

                // Change to third person camera
                OnChange();
            }
        }

        protected override void UpdateExit(GameTime deltaTime)
        {
            base.UpdateExit(deltaTime);

            if (DistanceToTarget < 1)
                m_CanChange = true;
        }

        protected override void Move(GameTime deltaTime)
        {
            base.Move(deltaTime);

            if (DistanceToTarget < 1)
                m_CanChange = true;
        }

        protected override void Rotate(GameTime deltaTime)
        {
            bool update = false;

            if (((m_CameraState == CameraState.Active) && (m_RotateAmount < 1)) ||
                ((m_CameraState == CameraState.Exit) && (m_RotateAmount > 0)))
                update = true;

            if (update)
            {
                // Save the rotate amount as the distance covered in a percentage divided by 100
                m_RotateAmount = (m_PercentageDistanceCovered / 100);

                if (m_CameraState == CameraState.Exit)
                {
                    // Invert the rotate amount but not using negatives
                    m_RotateAmount = (1 - m_RotateAmount);
                }

                // Rotate amount cannot exceed 1
                m_RotateAmount = MathHelper.Clamp(m_RotateAmount, 0, 1);

                // interpolate between the start rotation and the desired rotation
                m_Rotation = Quaternion.Lerp(m_RotationOnEnter, m_DesiredRotation, m_RotateAmount);
            }
        }

        public override void SetFocus(IFocusable focus)
        {
            base.SetFocus(focus);
        }

        public override void OnChangedTo(BaseCamera previousCamera)
        {
            base.OnChangedTo(previousCamera);

            if (previousCamera != null)
            {
                if ((previousCamera.GetType() == typeof(Camera3D)) || (previousCamera.GetType().IsSubclassOf(typeof(Camera3D))))
                {
                    Camera3D camera3D = (previousCamera as Camera3D);

                    if (camera3D.Focus != null)
                    {
                        if (camera3D.Focus != null)
                            m_PreviousCameraFocus = camera3D.Focus;
                    }

                    // Set target position
                    m_PreviousCameraTargetPosition = camera3D.CurrentTargetPosition;

                    // Set the distance from the target
                    m_PreviousCameraDistance = camera3D.GetDistance();
 
                    // Store rotation of previous camera when entering
                    m_RotationOnEnter = camera3D.Rotation;
                    Rotation = m_RotationOnEnter;       
                }

                // Set to move back to the start position
                TweenTo(previousCamera.Transform.Translation, m_PositionToViewFrom, 0.0f);
            
                // Reset change amount
                m_RotateAmount = 0.0f;
            }
        }

        public override void OnChange()
        {
            base.OnChange();

            // Set to move back to the start position
            //TweenTo(m_Position, m_PositionOnEnter, 0.5f);
            CurrentTargetPosition = Position;
            Focus = m_PreviousCameraFocus;
        }

        #region Set / Gets
        public override Vector3 Target
        {
            get
            {
                if (Focus != null)
                {
                    Vector3 targetVec = base.Target;
                    
                    targetVec += (m_PositionOnEnter - m_PreviousCameraTargetPosition);

                    // Direction from camera to the target position
                    /*Vector3 offset = (CurrentTargetPosition - targetVec);
                    offset.Normalize();

                    // Offset by the saved distance
                    offset *= m_PreviousCameraDistance;*/

                    return targetVec; 
                }
                else
                    return m_UnfocusedTarget;
            }
            set
            {
                m_UnfocusedTarget = value;
            }
        }

        public Vector3 ViewPos { get { return m_PositionToViewFrom; } set { m_PositionToViewFrom = value; } }
        public Quaternion DesiredRotation { get { return m_DesiredRotation; } set { m_DesiredRotation = Quaternion.CreateFromYawPitchRoll(value.Y, value.X, value.Z);  } }
        #endregion
    }
}
