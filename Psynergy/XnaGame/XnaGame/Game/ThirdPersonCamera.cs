using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

// Main Library
using Psynergy;

// Event Library
using Psynergy.Camera;

// Event Library
using Psynergy.Events;

// Input Library
using Psynergy.Input;

// Graphics Library
using Psynergy.Graphics;

namespace XnaGame
{
    public class ThirdPersonCamera : ArcBallCamera, IRegister<ThirdPersonCamera>
    {
        #region Factory Property setting
        protected override void ClassProperties(Factory factory)
        {
            factory.RegisterFloat("minrotationy", "MinRotationY");
            factory.RegisterFloat("maxrotationy", "MaxRotationY");
            factory.RegisterFloat("mindistance", "MinDistance");
            factory.RegisterFloat("maxdistance", "MaxDistance");
            factory.RegisterFloat("heightoffset", "TerrainHeightOffset");

            base.ClassProperties(factory);
        }
        #endregion

        private float m_PreviousPitch = 0.0f;
        private float m_PreviousYaw = 0.0f;
        private float m_PreviousRoll = 0.0f;
        private Quaternion m_PreviousRotation = Quaternion.Identity;

        // Distance variables
        protected float m_DesiredDistance = 0.0f;

        public ThirdPersonCamera() : base("")
        {
        }

        public ThirdPersonCamera(String name) : base(name)
        {
        }

        public ThirdPersonCamera(String name, IFocusable3D target, float minRotationY, float maxRotationY, float minDistance, float maxDistance) : base(name)
        {
            Focus = target;

            m_MinRotationY = minRotationY;
            m_MaxRotationY = maxRotationY;

            // Set distance variables
            m_MinDistance = minDistance;
            m_MaxDistance = maxDistance;
        }

        public override void Initialise()
        {
            base.Initialise();

            // Camera Type
            m_CameraType = CameraType.eCameraType_ThirdPerson;

            // Move slower then arc ball so it lerps
            m_DistanceMove = 100.0f;

            // Clamp the distance to within the min and max distance values
            m_DesiredDistance = MathHelper.Clamp(m_DesiredDistance, m_MinDistance, m_MaxDistance);
        }

        public override void Reset()
        {
            base.Reset();

            // Clamp the distance to within the min and max distance values
            m_DesiredDistance = MathHelper.Clamp(m_DesiredDistance, m_MinDistance, m_MaxDistance);
        }

        protected override void UpdateExit(GameTime deltaTime)
        {
            base.UpdateExit(deltaTime);

            // For now, change to the world camera
            CameraManager.Instance.ChangeCamera("World");
        }

        protected override void Move(GameTime deltaTime)
        {
            base.Move(deltaTime);

            if (m_Distance != m_DesiredDistance)
            {
                // Lerp to desired distance
                if (m_Distance < m_DesiredDistance)
                {
                    m_Distance += (m_DistanceMove * (float)deltaTime.ElapsedGameTime.TotalSeconds);

                    if (m_Distance > m_DesiredDistance)
                        m_Distance = m_DesiredDistance;
                }
                else if (m_Distance > m_DesiredDistance)
                {
                    m_Distance -= (m_DistanceMove * (float)deltaTime.ElapsedGameTime.TotalSeconds);

                    if (m_Distance < m_DesiredDistance)
                        m_Distance = m_DesiredDistance;
                }
            }
        }

        protected override void UpdateMoveInput(GameTime deltaTime)
        {
            bool valueChanged = false;

            // distance values ( mouse and controller )
            if (InputHandle.IsGamePadConnected(PlayerIndex.One))
            {
                if (InputHandle.LeftBumperDown(PlayerIndex.One))
                {
                    m_DesiredDistance += (m_DistanceMove * (float)deltaTime.ElapsedGameTime.TotalSeconds);

                    valueChanged = true;
                }

                if (InputHandle.RightBumperDown(PlayerIndex.One))
                {
                    m_DesiredDistance -= (m_DistanceMove * (float)deltaTime.ElapsedGameTime.TotalSeconds);

                    valueChanged = true;
                }
            }

            if (!m_DisableMovement)
            {
                if (!valueChanged)
                {
                    Vector3 mouseDelta = InputHandle.MouseDelta;
                    m_DesiredDistance += (mouseDelta.Z * (m_DistanceMove * 0.1f));
                }
            }

            // Clamp the distance to within the min and max distance values
            m_DesiredDistance = MathHelper.Clamp(m_DesiredDistance, m_MinDistance, m_MaxDistance);
        }

        protected override void Rotate(GameTime deltaTime)
        {
            // Save current rotation
            m_PreviousPitch = m_Pitch;
            m_PreviousYaw = m_Yaw;
            m_PreviousRoll = m_Roll;
            m_PreviousRotation = transform.Rotation;

            // Now commit rotations
            base.Rotate(deltaTime);
        }

        public override void SetInstantFocus(IFocusable focus)
        {
            base.SetInstantFocus(focus);

            m_Distance = m_DesiredDistance;
        }

        #region Camera change functions
        public override void OnChange()
        {
            base.OnChange();
        }

        public override void OnChangedTo(BaseCamera previousCamera)
        {
            base.OnChangedTo(previousCamera);
            
            // Set camera transform to previous camera transform
            transform = previousCamera.transform;

            if ((previousCamera.GetType() == typeof(Camera3D)) || (previousCamera.GetType().IsSubclassOf(typeof(Camera3D))))
            {
                Camera3D preCamera3D = (previousCamera as Camera3D);

                // Set rotation from previous camera
                transform.Rotation = preCamera3D.transform.Rotation;

                // Set new focus if previous camera had one
                if (preCamera3D.Focus != null)
                    SetInstantFocus(preCamera3D.Focus);
            }

            CurrentTargetPosition = Target;
        }
        #endregion

        #region event handlers
        #endregion

        #region Set / Gets
        public float DesiredDistance { get { return m_DesiredDistance; } set { m_DesiredDistance = value; } }
        #endregion
    }
}
