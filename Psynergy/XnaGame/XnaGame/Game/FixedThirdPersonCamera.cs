using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

/* Main Library */
using Psynergy;

namespace XnaGame
{
    class FixedThirdPersonCamera : ThirdPersonCamera, IRegister<FixedThirdPersonCamera>
    {
        #region Factory Property setting
        protected override void ClassProperties(Factory factory)
        {
            base.ClassProperties(factory);
        }
        #endregion

        private float m_DesiredPitch = 0.0f;
        private float m_PitchMove = 0.1f;

        public FixedThirdPersonCamera() : base("")
        {
        }

        public FixedThirdPersonCamera(String name) : base(name)
        {
        }

        public FixedThirdPersonCamera(String name, IFocusable3D target, float minRotationY, float maxRotationY, float minDistance, float maxDistance)
            : base(name, target, minRotationY, maxRotationY, minDistance, maxDistance)
        {
        }

        public override void Initialise()
        {
            base.Initialise();

            m_DisableMovement = true;
            MoveSpeed = 1.0f;
        }

        public override void Reset()
        {
            base.Reset();

            // Set desired pitch
            m_DesiredPitch = m_MinPitch;
            Pitch = m_MinPitch;
            m_DesiredDistance = m_MinDistance;
            m_Distance = m_DesiredDistance;
            Position = Vector3.Zero;
            CurrentTargetPosition = new Vector3(0, 50, 100);
        }

        public override void Load()
        {
            base.Load();
        }

        protected override void UpdateMoveInput(GameTime deltaTime)
        {
            base.UpdateMoveInput(deltaTime);
        }

        protected override void Rotate(GameTime deltaTime)
        {
            base.Rotate(deltaTime);

            float desiredInRadians = MathHelper.ToRadians(m_DesiredPitch);

            if (desiredInRadians != m_Pitch)
            {
                // Lerp to desired distance
                if (m_Pitch < desiredInRadians)
                {
                    m_Pitch += (m_PitchMove * (float)deltaTime.ElapsedGameTime.TotalSeconds);

                    if (m_Pitch > desiredInRadians)
                        m_Pitch = desiredInRadians;
                }
                else if (m_Pitch > desiredInRadians)
                {
                    m_Pitch -= (m_PitchMove * (float)deltaTime.ElapsedGameTime.TotalSeconds);

                    if (m_Pitch < desiredInRadians)         
                        m_Pitch = desiredInRadians;
                }
            }
        }

        public void IdleRotation(float speed)
        {
            m_Yaw += speed;
        }

        public bool FollowRotation(float rotY, float delta)
        {
            bool toRet = false;
            rotY = ClampWithin0to360(rotY);

            Vector3 startY = new Vector3(0.0f, m_Yaw, 0.0f);
            Vector3 desiredY = new Vector3(0.0f, rotY, 0.0f);
            Vector3 newY = new Vector3(0.0f, m_Yaw, 0.0f);

            float rotSpeed = (0.05f * delta);

            if (startY.Y > desiredY.Y)
                newY = Vector3.Lerp(startY, desiredY, rotSpeed);
            else if (startY.Y < desiredY.Y)
                newY = Vector3.Lerp(startY, desiredY, -rotSpeed);

            // Set yaw appropiately
            m_Yaw = newY.Y;

            Vector3 difference = (startY - desiredY);
            float tolerance = 0.1f;

            if ((difference.Y > tolerance) && (difference.Y < -tolerance))
                toRet = true;

            // Whether to generate a new value or not
            return toRet;
        }

        public float GetDistanceFromDesiredZoom()
        {
            return (m_Distance - m_DesiredDistance);
        }

        #region Set / Gets
        public float DesiredPitch { get { return m_DesiredPitch; } set { m_DesiredPitch = value; } }
        #endregion
    }
}
