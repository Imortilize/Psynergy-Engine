using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Psynergy;
using Psynergy.Graphics;
using Psynergy.Graphics.Terrain;
using Psynergy.AI;
using Psynergy.Input;

namespace Middleware
{
    public class ManualDriveController : GameNodeController, IRegister<ManualDriveController>
    {
        #region Fields
        #endregion

        #region Constructors
        public ManualDriveController() : base()
        {
        }

        public ManualDriveController(ModelNode node) : base(node)
        {
        }
        #endregion

        #region Functions
        protected override void UpdateMovement(GameTime deltaTime, Vector3 position)
        {
            Node3D node = (m_ObjReference as Node3D);
            if (node != null)
            {
                // If the node is currently focused.
                if (node.Focused)
                {
                    float delta = (float)deltaTime.ElapsedGameTime.TotalSeconds;

                    if (InputHandle.GetKey(Keys.W))
                        Velocity += (node.transform.WorldMatrix.Forward * m_MovementSpeed * delta);

                    if (InputHandle.GetKey(Keys.S))
                        Velocity += (node.transform.WorldMatrix.Backward * m_MovementSpeed * delta);
                }   
            }

            base.UpdateMovement(deltaTime, position);
        }

        protected override void UpdateRotation(GameTime deltaTime)
        {
            base.UpdateRotation(deltaTime);

            if (m_ObjReference != null)
            {
                Node3D node = (m_ObjReference as Node3D);
                if (node != null)
                {
                    // Current position
                    Vector3 newPos = node.transform.Position;

                    // Current rotation
                    Quaternion newRot = node.transform.Rotation;

                    // If the node is currently focused.
                    if (node.Focused)
                    {
                        float delta = (float)deltaTime.ElapsedGameTime.TotalSeconds;

                        // Check for manual rotation
                        if (InputHandle.GetKey(Keys.D) || InputHandle.GetKey(Keys.A))
                        {
                            float rotSpeed = ROTATION_SPEED;
                            if (InputHandle.GetKey(Keys.A))
                                rotSpeed *= -1;

                            Matrix rotMatrix = Matrix.CreateFromQuaternion(newRot);
                             
                            // Rotate by 90 degrees on the y axis
                            float rotationValue = (rotSpeed * delta);
                            float rotationInRadians = MathHelper.ToRadians(rotationValue);
                            Matrix rotationToApply = Matrix.CreateRotationY(rotationInRadians);

                            // Apply the rotation to the current rotation
                            rotMatrix *= rotationToApply;

                            // Convert the new matrix rotation to a quarternion and normalise it to rotation angles
                            // that can be used again without overflow.
                            newRot = Quaternion.CreateFromRotationMatrix(rotMatrix);
                            newRot.Normalize();
                        }

                        // Set the rotation
                        node.transform.Rotation = newRot;
                    }
                }
            }
        }
              
        public override void SetDesiredPosition(Vector3 desiredPos)
        {
        }

        public override void SetDesiredRotation(Vector3 from, Vector3 to)
        {
        }

        public override void StopMovement()
        {
        }
        #endregion
    }
}
