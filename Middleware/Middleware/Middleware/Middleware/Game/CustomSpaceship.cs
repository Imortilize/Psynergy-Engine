using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Psynergy;
using Psynergy.Graphics;
using Psynergy.Graphics.Terrain;
using Psynergy.Input;

namespace Middleware
{
    public class CustomSpaceship : ModelNode, IRegister<CustomSpaceship>
    {
        public CustomSpaceship()
            : base("")
        {

        }

        public CustomSpaceship(String name)
            : base(name)
        {

        }

        public override void Update(GameTime deltaTime)
        {
 	        base.Update(deltaTime);

            Vector3 newPos = m_Position;
            Quaternion newRot = m_Rotation;

            if (this.Focused)
            {
                float delta = (float)deltaTime.ElapsedGameTime.TotalSeconds;

                if (InputManager.Instance.KeyDown(Keys.W))
                    newPos += (WorldMatrix.Forward * 300.0f * delta);

                if (InputManager.Instance.KeyDown(Keys.S))
                    newPos += (WorldMatrix.Backward * 300.0f * delta);

                if (InputManager.Instance.KeyDown(Keys.D) || InputManager.Instance.KeyDown(Keys.A))
                {
                    float rotSpeed = 90;

                    if (InputManager.Instance.KeyDown(Keys.A))
                        rotSpeed *= -1;

                    Matrix rotMatrix = Matrix.CreateFromQuaternion(newRot);

                    // Rotate by 90 degrees on the y axis
                    rotMatrix *= Matrix.CreateRotationY(MathHelper.ToRadians(rotSpeed * delta));

                    // Save new rotation as a quarternion
                    newRot = Quaternion.CreateFromRotationMatrix(rotMatrix);
                    newRot.Normalize();
                }
            }

            Terrain terrain = TerrainManager.Instance.Terrain;

            if ( terrain != null )
            {
                if (terrain.TerrainInfo != null)
                {
                    float scaleAverage = ((terrain.Scale.X + terrain.Scale.Y + terrain.Scale.Z) / 3);

                    if (terrain.TerrainInfo.IsOnHeightmap(newPos / scaleAverage))
                    {
                        // We scale the position by the scale of the model so it is in the correct coordinate
                        // system as the terrain which is at full size still
                        Vector3 scalePos = (newPos / scaleAverage);
                        float height = newPos.Y;
                        Vector3 normal = Vector3.Up;

                        // Get the output height and normals of the position of the model
                        terrain.TerrainInfo.GetHeightAndNormal(scalePos, out height, out normal);

                        // Scale the height accordingly to come back into the correct coordinate system
                        height *= scaleAverage;

                        // Adjust the height by the position at which the terrain is currently set at
                        height += terrain.Position.Y;

                        // Set the final position of the model
                        newPos = new Vector3(newPos.X, (height + 20), newPos.Z);

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

            SetPos(newPos);
            m_Rotation = newRot;
        }
    }
}
