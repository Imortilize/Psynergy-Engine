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
    public class ChaseCamera : Camera3D
    {
        private Vector3 m_FollowTargetPosition = Vector3.Zero;
        private Quaternion m_FollowTargetRotation = Quaternion.Identity;
        private Vector3 m_PositionOffset = Vector3.Zero;
        private Vector3 m_TargetOffset = Vector3.Zero;
        private Quaternion m_RelativeCameraRotation = Quaternion.Identity;
        private float m_Springiness = 0.025f;

        public ChaseCamera() : base("")
        {
        }

        public ChaseCamera(String name)
            : base(name)
        {
        }

        public ChaseCamera(String name, IFocusable3D target, Vector3 positionOffset, Vector3 targetOffset, Quaternion relativeCameraRotation)
            : base(name)
        {
            Focus = target;
            PositionOffset = positionOffset;
            TargetOffset = targetOffset;
            RelativeCameraRotation = relativeCameraRotation;
        }

        public override void Initialise()
        {
            base.Initialise();

            // Set that it does required focus
            RequiresFocus = true;
        }

        protected override void Move(GameTime deltaTime)
        {
            Debug.Assert( Focus != null, "[Warning] - Focus is null in Chase Camera '" + Name + "'.");

            if (Focus != null)
                FollowTargetPosition = Focus.transform.WorldMatrix.Translation;
        }

        protected override void Rotate(GameTime deltaTime)
        {
            if (Focus != null)
                FollowTargetRotation = Focus.transform.Rotation;
        }

        protected override void GenerateViewMatrix(GameTime deltaTime)
        {
            Debug.Assert(Focus != null, "[Warning] - Focus object is null on Arc Ball Camera '" + Name + "'.");

            if (Focus != null)
            {
                // Calculate the rotation matrix for the camera
                Matrix rotation = Matrix.CreateFromQuaternion(FollowTargetRotation * RelativeCameraRotation);

                // Calculate the position the camera would be without the spring value, using the rotation matrix and target position
                Vector3 desiredPosition = (FollowTargetPosition + Vector3.Transform(PositionOffset, rotation));

                // Calculate the new target using the rotation matrix
                Vector3 target = (FollowTargetPosition + Vector3.Transform(TargetOffset, rotation));

                // Interpolate between the current position and the desired position 
                transform.Position = Vector3.Lerp(transform.Position, desiredPosition, Springiness);

                // Obtain the up vector from the matrix
                Vector3 up = Vector3.Transform(Vector3.Up, rotation);

                // Recalculate the view matrix
                View = Matrix.CreateLookAt(transform.Position, target, up);
            }
        }

        #region Properties
            public Vector3 FollowTargetPosition { get { return m_FollowTargetPosition; } set { m_FollowTargetPosition = value; } }
            public Quaternion FollowTargetRotation { get { return m_FollowTargetRotation; } set { m_FollowTargetRotation = value; } }
            public Vector3 PositionOffset { get { return m_PositionOffset; } set { m_PositionOffset = value; } }
            public Vector3 TargetOffset { get { return m_TargetOffset; } set { m_TargetOffset = value; } }
            public Quaternion RelativeCameraRotation { get { return m_RelativeCameraRotation; } set { m_RelativeCameraRotation = value; } }
            public float Springiness { get { return m_Springiness; } set { m_Springiness = MathHelper.Clamp(value, 0, 1); } }
        #endregion
    }
}
