using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using Psynergy.Input;

namespace Psynergy.Camera
{
    public class ArcBallCamera : Camera3D, IRegister<ArcBallCamera>
    {
        #region Factory Property setting
        protected override void ClassProperties(Factory factory)
        {
            factory.RegisterFloat("minrotationy", "MinRotationY");
            factory.RegisterFloat("maxrotationy", "MaxRotationY");
            factory.RegisterFloat("minpitch", "MinPitch");
            factory.RegisterFloat("maxpitch", "MaxPitch");
            factory.RegisterFloat("mindistance", "MinDistance");
            factory.RegisterFloat("maxdistance", "MaxDistance");

            base.ClassProperties(factory);
        }
        #endregion

        protected bool m_DisableMovement = false;
        protected float m_MinRotationY = 0.0f;
        protected float m_MaxRotationY = 0.0f;
        protected float m_MinPitch = 0f;
        protected float m_MaxPitch = 360f;
        protected float m_Distance = 0.0f;
        protected float m_MinDistance = 0.0f;
        protected float m_MaxDistance = 0.0f;
        protected float m_DistanceMove = 100.0f;



        // Test
        private float m_CameraRotation = 0.0f;
        private float m_CameraArc = 0.0f;
        private float m_CameraRotateSpeed = 0.1f;

        public ArcBallCamera() : base("")
        {
        }

        public ArcBallCamera(String name)
            : base(name)
        {
        }

        public ArcBallCamera(String name, IFocusable3D target, float minRotationY, float maxRotationY, float minDistance, float maxDistance)
            : base(name)
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

            // Set the distance to half way between the min and maximum rotationY as default
            //RotY = (MinRotationY + ((MaxRotationY - MinRotationY) / 2));

            // Set the x rotation value 
            m_Pitch = m_MaxPitch;

            // Set the distance to half way between the min and maximum distance as default
            //m_Distance = (m_MinDistance + ((m_MaxDistance - m_MinDistance) / 2));
            m_Distance = 150;

            // Set up the possible tweening speed
            MoveSpeed = 2.0f;

            // Allow tweeining
            m_StartTween = true;

            // Set that it does required focus
            RequiresFocus = true;
        }

        public override void Reset()
        {
            base.Reset();

            // Set the distance to half way between the min and maximum rotationY as default
            //RotY = (MinRotationY + ((MaxRotationY - MinRotationY) / 2));

            // Allow tweeining
            m_StartTween = true;
        }

        protected override void OnEnter()
        {
            base.OnEnter();

            m_Tween = true;
        }

        protected override void UpdateExit(GameTime deltaTime)
        {
            base.UpdateExit(deltaTime);
        }

        protected override void Move(GameTime deltaTime)
        {
            // Update movement input
            UpdateMoveInput(deltaTime);

            // Move base code
            base.Move(deltaTime);
        }

        protected virtual void UpdateMoveInput(GameTime deltaTime)
        {
            if (!m_DisableMovement)
            {
                bool valueChanged = false;

                // distance values ( mouse and controller )
                if (InputHandle.IsGamePadConnected(PlayerIndex.One))
                {
                    if (InputHandle.LeftBumperDown(PlayerIndex.One))
                    {
                        m_Distance += (m_DistanceMove * (float)deltaTime.ElapsedGameTime.TotalSeconds);

                        valueChanged = true;
                    }

                    if (InputHandle.RightBumperDown(PlayerIndex.One))
                    {
                        m_Distance -= (m_DistanceMove * (float)deltaTime.ElapsedGameTime.TotalSeconds);

                        valueChanged = true;
                    }
                }

                if (!valueChanged)
                {
                    Vector3 mouseDelta = InputHandle.MouseDelta;
                    m_Distance += mouseDelta.Z;
                }
            }

            // Clamp the distance to within the min and max distance values
            m_Distance = MathHelper.Clamp(m_Distance, m_MinDistance, m_MaxDistance);
        }

        protected override void Rotate(GameTime deltaTime)
        {
            if (!m_DisableMovement)
            {
              /*  float time = (float)deltaTime.ElapsedGameTime.TotalMilliseconds;


                // Check for input to rotate the camera up and down around the model.
                if (InputHandle.GetKey(Keys.Up) || InputHandle.GetKey(Keys.W))
                {
                    m_CameraArc += time * m_CameraRotateSpeed;
                }

                if (InputHandle.GetKey(Keys.Down) || InputHandle.GetKey(Keys.S))
                {
                    m_CameraArc -= time * m_CameraRotateSpeed;
                }*/



                // Reset the rotation values accordingly
                m_YawSpeed = 0.0f;
                m_PitchSpeed = 0.0f;

                bool valueChanged = false;

                // If the game pad is connected then use these values over the mouse
                if (InputHandle.IsGamePadConnected(PlayerIndex.One))
                {
                    Vector2 leftThumbStick = InputHandle.LeftStick(PlayerIndex.One);

                    // Left thumbstick
                    m_YawSpeed = (leftThumbStick.X * 5);
                    m_PitchSpeed = (leftThumbStick.Y * 5);

                    if ((leftThumbStick.X != 0) || (leftThumbStick.Y != 0))
                        valueChanged = true;
                }

                if (!valueChanged)
                {
                    // If right mouse is down allow rotation
                    if (InputHandle.GetMouse(1) || InputHandle.GetMouse(2))
                    {
                        // Get the mouse delta
                        Vector3 mouseDelta = InputHandle.MouseDelta;

                        // Apply accordingly
                        m_YawSpeed = (mouseDelta.X * 5);
                        m_PitchSpeed = (mouseDelta.Y * 5);
                    }
                }
            }

            // Update actual rotation values in base class
            base.Rotate(deltaTime);
        }

        protected override void Clamp()
        {
            // Clamp pitch to within the boundaries specified
            m_Pitch = MathHelper.Clamp(m_Pitch, MathHelper.ToRadians(m_MinPitch), MathHelper.ToRadians(m_MaxPitch));
        }

        protected override void GenerateViewMatrix(GameTime deltaTime)
        {
            //Debug.Assert(Focus != null, "[Warning] - Focus object is null on Arc Ball Camera '" + Name + "'.");

            if (Focus != null)
            {
             	// Calculate the rotation matrix
                Matrix rotation = Matrix.CreateFromQuaternion(transform.Rotation);
	
	            // Translate down the Z axis by the desired distance between the camera and object, then rotate that
	            // vector to find the camera offset from the target.
	            m_Translation = new Vector3(0, 0, Distance);
	            m_Translation = Vector3.Transform(m_Translation, rotation);
	
	            // Set the new position
                Vector3 newPos = (CurrentTargetPosition + m_Translation);
	
	            // Check this position
	            newPos = PositionChecks(newPos);
	
	            // Set the new position
                transform.Position = newPos;

                // Create Arc Ball camera matrix
                View = Matrix.CreateFromQuaternion(transform.Rotation) * Matrix.CreateTranslation(transform.Position);

                // Calculate the rotation matrix
                /*Matrix rotation = Matrix.CreateFromQuaternion(Rotation);

                // Translate down the Z axis by the desired distance between the camera and object, then rotate that
                // vector to find the camera offset from the target.
                m_Translation = new Vector3(0, 0, Distance);
                m_Translation = Vector3.Transform(m_Translation, rotation);

                // Set the new position
                Vector3 newPos = (CurrentTargetPosition + m_Translation);

                // Check this position
                newPos = PositionChecks(newPos);

                // Set the new position
                Position = newPos;*/

                //Matrix unrotatedView = Matrix.CreateLookAt(new Vector3(0, 0, -Distance), Vector3.Zero, Vector3.Up);
                //Matrix rot = Matrix.CreateRotationY(Yaw) * Matrix.CreateRotationX(Pitch);
                //Transform = Matrix.CreateTranslation(-Target) * rot * unrotatedView;
                //Transform = rot * unrotatedView;
                // Store position for now
                //Position = Transform.Translation;
            }
        }

        protected virtual Vector3 PositionChecks( Vector3 pos )
        {
            return pos;
        }

        public override float GetDistance()
        {
            return Distance;
        }

        #region Camera change functions
        public override void OnChange()
        {
            base.OnChange();
        }

        public override void OnChangedTo(BaseCamera previousCamera)
        {
            base.OnChangedTo(previousCamera);
        }
        #endregion

        #region Properties
        // Y axis rotation limits (radians)
        public float MinRotationY { get { return m_MinRotationY; } set { m_MinRotationY = value;  } }
        public float MaxRotationY { get { return m_MaxRotationY; } set { m_MaxRotationY = value; } }

        // Pitch rotation limits (radians)
        public float MinPitch { get { return m_MinPitch; } set { m_MinPitch = value; } }
        public float MaxPitch { get { return m_MaxPitch; } set { m_MaxPitch = value; } }

        // Distance between the target and camera
        public float Distance { get { return m_Distance; } set { m_Distance = value; } }

        // Distance limits
        public float MinDistance { get { return m_MinDistance; } set { m_MinDistance = value; } }
        public float MaxDistance { get { return m_MaxDistance; } set { m_MaxDistance = value; } }
        #endregion
    }
}
