using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

/* Input Library */
using Psynergy.Input;

namespace Psynergy.Camera
{
    public class FreeCamera : Camera3D, IRegister<FreeCamera>
    {
        public FreeCamera() : base("")
        {
        }

        public FreeCamera(String name) : base(name)
        {
        }

        public FreeCamera(String name, Vector3 startPositon, Quaternion startRotation) : base(name)
        {
            // Position
            m_Position = startPositon;

            // Rotation
            Pitch = startRotation.X;
            Yaw = startRotation.Y;
            Roll = startRotation.Z;
            m_Rotation = Quaternion.CreateFromYawPitchRoll(Yaw, Pitch, Roll);
        }

        public override void Initialise()
        {
            base.Initialise();

            // Set that it doesn't required focus
            RequiresFocus = false;
            MoveSpeed = 2.0f;
        }

        protected override void Move(GameTime deltaTime)
        {
            // Translation values
            if (InputHandle.GetKeyDown(Keys.W)) 
                m_Translation += (Vector3.Forward * MoveSpeed);

            if (InputHandle.GetKeyDown(Keys.A))
                m_Translation += (Vector3.Left * MoveSpeed);

            if (InputHandle.GetKeyDown(Keys.S)) 

                m_Translation += (Vector3.Backward * MoveSpeed);
            if (InputHandle.GetKeyDown(Keys.D)) 
                m_Translation += (Vector3.Right * MoveSpeed);

            base.Move(deltaTime);
        }

        protected override void Rotate(GameTime deltaTime)
        {
            // Rotation values
            if (InputHandle.GetMouse(1))
            {
                Vector3 mouseDelta = InputHandle.MouseDelta;

                m_YawSpeed = (mouseDelta.X * 5f);
                m_PitchSpeed = (mouseDelta.Y * 5f);
            }
            else
            {
                m_YawSpeed = 0.0f;
                m_PitchSpeed = 0.0f;
                m_RollSpeed = 0.0f;
            }

            // Update actual rotation values in base class
            base.Rotate(deltaTime);
        }

        protected override void GenerateViewMatrix(GameTime deltaTime)
        {
            // Fetch the rotation matrix;
            Matrix rotation = Matrix.CreateFromQuaternion(Rotation);

            // Offset the position and reset the translation
            m_Translation = Vector3.Transform(m_Translation, rotation);

            // Set the new position
            Position = (CurrentTargetPosition + m_Translation);
            m_Translation = Vector3.Zero;

            // Calculate the new target
            //Vector3 forward = Vector3.Transform(Vector3.Forward, rotation);
           // Vector3 target = (Position + forward);

            // Calculate the up vector
            //Vector3 up = Vector3.Transform(Vector3.Up, rotation);

            // Calculate the view matrix
            //View = Matrix.CreateLookAt(Position, target, up);

            Transform = Matrix.CreateFromQuaternion(Rotation) * Matrix.CreateTranslation(Position);
        }
    }
}
