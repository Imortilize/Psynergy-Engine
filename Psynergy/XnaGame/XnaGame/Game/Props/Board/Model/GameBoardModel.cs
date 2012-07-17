using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

/* Main Library */
using Psynergy;

/* Graphics Library */
using Psynergy.Graphics;

namespace XnaGame
{
    class GameBoardModel : ModelNode
    {
        #region Factory Property setting
        protected override void ClassProperties(Factory factory)
        {


            base.ClassProperties(factory);
        }
        #endregion

        public GameBoardModel() : base("")
        {
        }

        public GameBoardModel(String name) : base(name)
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

        public override void UnLoad()
        {
            base.UnLoad();
        }

        public override void Reset()
        {
            base.Reset();
        }

        public override void Update(GameTime deltaTime)
        {
            base.Update(deltaTime);
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
