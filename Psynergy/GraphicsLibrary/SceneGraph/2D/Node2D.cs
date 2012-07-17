using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

/* Main Library */
using Psynergy;

namespace XnaGame
{
    public class Node2D : RenderNode
    {
        #region Factory Property setting
        protected override void ClassProperties(Factory factory)
        {
            base.ClassProperties(factory);
        }
        #endregion

        protected float m_Rotation = 0.0f;
        protected float m_StartRotation = 0.0f;

        public Node2D() : base("")
        {
        }

        public Node2D(String name)
            : base(name)
        {
        }

        public override void Initialise()
        {
            // Set the start rotation
            m_StartRotation = Rotation;

            base.Initialise();
        }

        public override void Reset()
        {
            base.Reset();

            Rotation = m_StartRotation;
        }

        public override void Update(GameTime deltaTime)
        {
           base.Update(deltaTime);

           // Update model rotation values
           UpdateRotation(deltaTime);
        }

        private void UpdateRotation(GameTime deltaTime)
        {
            // TODO:
        }

        public override void Render(GameTime deltaTime)
        {
            base.Render(deltaTime);
        }

        #region Properties
        public float Rotation { get { return m_Rotation; } set { m_Rotation = value; } }
        #endregion
    }
}
