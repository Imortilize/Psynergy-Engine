using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using LTreesLibrary.Trees;

using Psynergy.Camera;

namespace Psynergy.Graphics
{
    public class LTree : Node3D
    {
        private SimpleTree m_SimpleTree = null;

        public LTree() : base("")
        {
        }

        public LTree(SimpleTree tree) : base("")
        {
            m_SimpleTree = tree;
        }

        public LTree(String name, SimpleTree tree) : base(name)
        {
            m_SimpleTree = tree;
        }

        public override void Initialise()
        {
            base.Initialise();

            RenderGroupName = "shadow";
        }

        public override void Load()
        {
            base.Load();
        }

        public override void Reset()
        {
            base.Reset();
        }

        public override void Update(GameTime deltaTime)
        {
            base.Update(deltaTime);
        }

        public override void Render(GameTime deltaTime)
        {
            base.Render(deltaTime);

            /*if (m_SimpleTree != null)
            {
                Camera3D camera3D = (CameraManager.Instance.ActiveCamera as Camera3D);

                if ( camera3D != null )
                {
                    // Draw tree trunk
                    m_SimpleTree.DrawTrunk(WorldMatrix, camera3D.View, camera3D.Projection);

                    // Draw tree leaves
                    m_SimpleTree.DrawLeaves(WorldMatrix, camera3D.View, camera3D.Projection);
                }
            }*/
        }
    }
}
