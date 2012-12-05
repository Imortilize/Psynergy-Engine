using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

/* Main Library */
using Psynergy;

/* Graphics Library */
using Psynergy.Graphics;

/* Input Library */
using Psynergy.Input;

namespace XnaGame
{
    class Player2D : SpriteNode, IFocusable
    {
        private float m_Acceleration = 15.0f;
        private float m_JumpAcceleration = -5.0f;
        private bool m_CanJump = true;

        private Vector3 m_MovementAcceleration = new Vector3(0.0f, 0.0f, 0.0f);
        private Vector3 m_MaximumMovementAcceleration = new Vector3(8.0f, 30.0f, 0.0f);
        private Vector3 m_MovementSpeed = new Vector3(0.0f, 0.0f, 0.0f);
        private Vector3 m_MaximumMovementSpeed = new Vector3(10.0f, 30.0f, 0.0f);

        public Player2D() : base()
        {
        }

        public Player2D(String name, String assetName, Vector3 position) : base(name, assetName, position)
        {
        }

        public override void Initialise()
        {
            // TODO...
            m_Weight = 1.5f;

            base.Initialise();
        }

        public override void Reset()
        {
            m_CanJump = true;
        }

        public override void Load()
        {
            base.Load();

            // After loading
            if ((PosY + Height) > 0)
                PosY = (0 - Height);
        }

        public override void Update(GameTime deltaTime)
        {
            if (InputHandle.GetKey(Keys.D))
                Move(m_Acceleration);
            else if (InputHandle.GetKey(Keys.A))
                Move(-m_Acceleration);
            else
                Move(0.0f);

            // If the player is allowed to jump
            if (m_CanJump)
            {
                if (InputHandle.GetKey(Keys.Space))
                    Jump();
            }

            // Update the player movement
            UpdateMovement(deltaTime);

            base.Update(deltaTime);
        }

        private void Move(float movementFactor)
        {
            m_MovementAcceleration.X = movementFactor;
        }

        private void Jump()
        {
            m_MovementSpeed.Y = m_JumpAcceleration;

            if (m_MovementSpeed.Y > m_MaximumMovementSpeed.Y)
                m_MovementSpeed.Y = m_MaximumMovementSpeed.Y;
            else if (m_MovementSpeed.Y < -m_MaximumMovementSpeed.Y)
                m_MovementSpeed.Y = -m_MaximumMovementSpeed.Y;

            // Set to a free falling state
            m_CanJump = false;
        }

        private void UpdateMovement(GameTime deltaTime)
        {
            // Apply movement speed
            m_MovementSpeed += (m_MovementAcceleration * (float)deltaTime.ElapsedGameTime.TotalSeconds);

            // Apply gravity
            ApplyGravity(deltaTime);

            // Apply drag
            ApplyDrag(deltaTime);

            // Clamp movement speed
            ClampMovementSpeed();

            // Set player position
            Vector3 newPos = (Position + m_MovementSpeed);

            // Set the position
            Position = newPos;
        }

        private void ClampMovementSpeed()
        {
            // Clamp Acceleration
            // X
            if (m_MovementAcceleration.X > m_MaximumMovementAcceleration.X)
                m_MovementAcceleration.X = m_MaximumMovementAcceleration.X;
            else if (m_MovementAcceleration.X < -m_MaximumMovementAcceleration.X)
                m_MovementAcceleration.X = -m_MaximumMovementAcceleration.X;

            // Clamp speeds
            // X
            if (m_MovementSpeed.X > m_MaximumMovementSpeed.X)
                m_MovementSpeed.X = m_MaximumMovementSpeed.X;
            else if (m_MovementSpeed.X < -m_MaximumMovementSpeed.X)
                m_MovementSpeed.X = -m_MaximumMovementSpeed.X;

            // Y
            if (m_MovementSpeed.Y > m_MaximumMovementSpeed.Y)
                m_MovementSpeed.Y = m_MaximumMovementSpeed.Y;
            else if (m_MovementSpeed.Y < -m_MaximumMovementSpeed.Y)
                m_MovementSpeed.Y = -m_MaximumMovementSpeed.Y;
        }

        private void ApplyGravity(GameTime deltaTime)
        {
            if (m_MovementSpeed.Y < GRAVITY)
                m_MovementSpeed.Y += (GRAVITY * m_Weight * (float)deltaTime.ElapsedGameTime.TotalSeconds);

            // Check if the player has hit any obstacles free falling
            if ((PosY + Height) > 0)
            {
                PosY = (0 - Height);

                m_MovementSpeed.Y = 0;
                m_MovementAcceleration.Y = 0;
                m_CanJump = true;
            }
        }

        private void ApplyDrag(GameTime deltaTime)
        {
            // Apply friction or air resistance ( depending if they are in the air or not )
            if (m_MovementSpeed.X != 0)
            {
                float drag = 0.0f;

                if (m_MovementAcceleration.Y <= 0)
                    drag = FRICTION;
                else
                    drag = AIRRESISTANCE;

                if (m_MovementSpeed.X >= 0)
                {
                    m_MovementSpeed.X -= (drag * m_Weight * (float)deltaTime.ElapsedGameTime.TotalSeconds);

                    if (m_MovementSpeed.X < 0)
                        m_MovementSpeed.X = 0;
                }
                else if (m_MovementSpeed.X < 0)
                {
                    m_MovementSpeed.X += (drag * m_Weight * (float)deltaTime.ElapsedGameTime.TotalSeconds);

                    if (m_MovementSpeed.X > 0)
                        m_MovementSpeed.X = 0;
                }
            }
        }

        public Vector3 MovementSpeed { get { return m_MovementAcceleration; } set { m_MovementAcceleration = value; } }
        public IFocusable2D Focus { get { return (this as IFocusable2D); } }
    }
}
