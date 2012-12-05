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
using Psynergy.Camera;

namespace Middleware
{
    public class GameNodeController : Controller, IRegister<GameNodeController>
    {
        #region Fields
        // Desired position
        private Vector3? m_DesiredPosition = null;

        // Desired rotation
        protected Quaternion? m_DesiredRotation = null;
        protected float m_RotationVelocity = 4.0f;

        // Terrain values
        protected float m_TerrainScaledAverage = 1.0f;
        protected Vector3 m_ScaledPosition = Vector3.Zero;

        // Rotation Speed
        protected const float ROTATION_SPEED = 90.0f;

        // Offset from the terrain
        protected const float FLOAT_DISTANCE = 20.0f;
        #endregion

        #region Constructors
        public GameNodeController()
            : base()
        {
        }

        public GameNodeController(Node node)
            : base(node)
        {
        }
        #endregion

        #region Functions
        public override void Reset()
        {
            base.Reset();

            // Stop movement 
            StopMovement();
        }

        public override void Update(GameTime deltaTime)
        {
            base.Update(deltaTime);

            // Update this node to sit above the terrain if one exists
            UpdateTerrainTransform(deltaTime);
        }

        protected override void UpdateMovement(GameTime deltaTime, Vector3 position)
        {
            // If a desired position exists, then we move towards it
            if (m_DesiredPosition != null)
            {
                BaseCamera camera = CameraManager.Instance.ActiveCamera;
                if (camera != null)
                {
                    float delta = (float)deltaTime.ElapsedGameTime.TotalSeconds;

                    // Position direction
                    Vector3 direction = (m_DesiredPosition.Value - m_ObjReference.Position);
                    direction.Normalize();

                    // Apply the velocity
                    Velocity = (direction * m_MovementSpeed * delta);

                    DebugRender.Instance.AddNewDebugLineGroup(m_ObjReference.Position, Color.Blue);
                    DebugRender.Instance.AddDebugLine(m_DesiredPosition.Value, Color.Red);
                    DebugRender.Instance.AddDebugLine(m_DesiredPosition.Value, Color.Red);
                    //DebugRender.Instance.DrawDebugPoint(m_DesiredPosition.Value, 10);

                    DebugRender.Instance.AddNewDebugLineGroup(Vector3.One, Color.Yellow);
                    DebugRender.Instance.AddNewDebugLineGroup(-camera.Transform.Translation, Color.Yellow);
                    DebugRender.Instance.AddDebugLine(m_DesiredPosition.Value, Color.Orange);
                    DebugRender.Instance.AddDebugLine(m_DesiredPosition.Value, Color.Orange);
                }
            }

            base.UpdateMovement(deltaTime, position);
        }

        protected override void UpdateRotation(GameTime deltaTime)
        {
            base.UpdateRotation(deltaTime);

            if (m_DesiredRotation != null)
            {
                Node3D node = (m_ObjReference as Node3D);

                // Update the rotation
                node.Rotation = Quaternion.Slerp(node.Rotation, m_DesiredRotation.Value, (m_RotationVelocity * (float)deltaTime.ElapsedGameTime.TotalSeconds));

                // increase rotation
                //node.OrbitalPitch += (node.OrbitalPitchSpeed * (float)deltaTime.ElapsedGameTime.TotalSeconds);
               // node.OrbitalYaw += (node.OrbitalYawSpeed * (float)deltaTime.ElapsedGameTime.TotalSeconds);
               // node.OrbitalRoll += (node.OrbitalRollSpeed * (float)deltaTime.ElapsedGameTime.TotalSeconds);
            }
        }

        private void UpdateTerrainTransform(GameTime deltaTime)
        {
            if (m_ObjReference != null)
            {
                Node3D node = (m_ObjReference as Node3D);
                if (node != null)
                {
                    // Current position
                    Vector3 newPos = node.Position;

                    // Current rotation
                    Quaternion newRot = node.Rotation;

                    // See if a terrain exists
                    Terrain terrain = TerrainManager.Instance.Terrain;
                    if (terrain != null)
                    {
                        if (terrain.TerrainInfo != null)
                        {
                            m_TerrainScaledAverage = terrain.AverageScale;

                            // We scale the position by the scale of the model so it is in the correct coordinate
                            // system as the terrain which is at full size still
                            m_ScaledPosition = (newPos / m_TerrainScaledAverage);

                            // Check if the object is on the heightmap or not
                            if (terrain.TerrainInfo.IsOnHeightmap(m_ScaledPosition))
                            {
                                float height = 0.0f;
                                Vector3 normal = Vector3.Zero;

                                // Get the relevant terrain information using the scaled position
                                terrain.TerrainInfo.GetHeightAndNormal(m_ScaledPosition, out height, out normal);

                                // Scale the height accordingly to come back into the correct coordinate system
                                height *= m_TerrainScaledAverage;

                                // Adjust the height by the position at which the terrain is currently set at
                                height += terrain.Position.Y;

                                // Set the final position of the model
                                newPos = new Vector3(newPos.X, (height + FLOAT_DISTANCE), newPos.Z);

                                // Now we use the normal to calculate the final rotation
                                Matrix rotMatrix = Matrix.CreateFromQuaternion(newRot);

                                // Calculate orientation rotations
                                rotMatrix.Up = normal;

                                rotMatrix.Right = Vector3.Cross(rotMatrix.Forward, rotMatrix.Up);
                                rotMatrix.Right = Vector3.Normalize(rotMatrix.Right);

                                rotMatrix.Forward = Vector3.Cross(rotMatrix.Up, rotMatrix.Right);
                                rotMatrix.Forward = Vector3.Normalize(rotMatrix.Forward);

                                // Convert back to quarternion
                                newRot = Quaternion.CreateFromRotationMatrix(rotMatrix);
                            }
                        }
                    }

                    // Set the new position
                    node.Position = newPos;

                    // Set the new rotation
                    node.Rotation = newRot;
                }
            }
        }

        public override Vector3 SetPosition(Vector3 position)
        {
            base.SetPosition(position);

            // Stop any prior movement
            StopMovement();

            // Return the new position
            return position;
        }

        public override void SetDesiredPosition(Vector3 desiredPos)
        {
            base.SetDesiredPosition(desiredPos);

            // Set the desired position
            m_DesiredPosition = desiredPos;

            // Set the rotation to follow it in
            SetDesiredRotation(m_ObjReference.Position, desiredPos);
        }

        public override void SetDesiredRotation(Vector3 from, Vector3 to)
        {
            base.SetDesiredRotation(from, to);

            if (from != to)
            {
                Vector3 angles = MathLib.Instance.AngleTo(from, to);

                m_DesiredRotation = (Quaternion.CreateFromAxisAngle(Vector3.Up, angles.Y) * Quaternion.CreateFromAxisAngle(Vector3.Right, angles.X));
            }
        }

        public override void StopMovement()
        {
            base.StopMovement();

            // No desired position
            m_DesiredPosition = null;
            m_DesiredRotation = null;
        }
        #endregion
    }
}
