using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

/* Physics library */
using Psynergy.Physics;

namespace Psynergy.Graphics
{
    class CustomDice : CubeNode, PhysicsActor
    {
        public CustomDice() : base()
        {
        }

        public CustomDice(String name) : base(name)
        {
        }

        public override void Initialise()
        {
            base.Initialise();
        }

        public override void Load()
        {
            base.Load();
        }

        protected override void LoadSpecific()
        {
            base.LoadSpecific();
        }

        public override void Update(GameTime deltaTime)
        {
            base.Update(deltaTime);
        }

        public override void Render(GameTime deltaTime)
        {
            base.Render(deltaTime);
        }

        public virtual void Enable()
        {
            ActiveRender = true;
        }

        public virtual void Disable()
        {
            ActiveRender = false;
        }
    }
}
