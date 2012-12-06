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
    public class TestController : GameNodeController, IRegister<TestController>
    {
        #region Fields
        #endregion

        #region Constructors
        public TestController()
            : base()
        {
        }

        public TestController(ModelNode node)
            : base(node)
        {
        }
        #endregion

        #region Functions
        public override void Update(GameTime deltaTime)
        {
            if (m_ObjReference.Focused)
            {
                // Check for terrain picking
                if (InputHandle.GetMouse(0))
                {
                    // Test
                    Terrain terrain = TerrainManager.Instance.Terrain;
                    if (terrain != null)
                    {
                        Vector3? terrainPick = terrain.Pick();
                        if (terrainPick != null)
                        {
                            // For now just set the ship model to this position for testing
                            SetDesiredPosition(terrainPick.Value);
                        }
                    }
                }
            }
            else
            {
                ModelNode model = (m_ObjReference as ModelNode);
                if (model != null)
                {
                    if (InputHandle.GetMouse(2))
                    {
                        BaseCamera camera = CameraManager.Instance.ActiveCamera;
                        if (camera != null)
                        {
                            Ray ray = camera.CastRay(Matrix.Identity);

                            // Cast the ray against the mesh
                            if (model.Intersects(ray) != null)
                            {
                                // Set this node as the node to focus
                                camera.SetFocus(model);
                            }
                        }
                    }
                }
            }


            base.Update(deltaTime);
        }

        protected override void UpdateMovement(GameTime deltaTime, Vector3 position)
        {
            ModelNode model = (m_ObjReference as ModelNode);

            //  DebugRender.Instance.AddNewDebugLineGroup(m_ObjReference.Position, Color.Blue);

            float halfBoundingRadius = model.BoundingSphere.Radius;
            Vector3 right = model.transform.WorldMatrix.Right;
            Vector3 left = model.transform.WorldMatrix.Left;

            DebugRender.Instance.AddNewDebugLineGroup(m_ObjReference.transform.Position, Color.Blue);
            DebugRender.Instance.AddDebugLine((m_ObjReference.transform.Position + (right * halfBoundingRadius)), Color.Red);
            DebugRender.Instance.AddDebugLine((m_ObjReference.transform.Position + (right * halfBoundingRadius)), Color.Red);

            DebugRender.Instance.AddNewDebugLineGroup(m_ObjReference.transform.Position, Color.Blue);
            DebugRender.Instance.AddDebugLine((m_ObjReference.transform.Position + (left * halfBoundingRadius)), Color.Red);
            DebugRender.Instance.AddDebugLine((m_ObjReference.transform.Position + (left * halfBoundingRadius)), Color.Red);

            DebugRender.Instance.AddNewDebugLineGroup(m_ObjReference.transform.Position, Color.Orange);
            DebugRender.Instance.AddDebugLine(m_ObjReference.transform.Position + (model.transform.WorldMatrix.Forward * (Velocity.Length() * 100)), Color.Orange);
            DebugRender.Instance.AddDebugLine(m_ObjReference.transform.Position + (model.transform.WorldMatrix.Forward * (Velocity.Length() * 100)), Color.Orange);

            // Run base class movement code
            base.UpdateMovement(deltaTime, position);
        }

        protected override void UpdateRotation(GameTime deltaTime)
        {
            base.UpdateRotation(deltaTime);
        }
        #endregion
    }
}
