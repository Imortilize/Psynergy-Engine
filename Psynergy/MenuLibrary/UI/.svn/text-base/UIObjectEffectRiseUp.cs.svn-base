using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Psynergy.Menus
{
    class UIObjectEffectRiseUp : UIObjectEffect
    {
        private float m_RiseRate = 0.0f;

        public UIObjectEffectRiseUp( float riseRate ) : base()
        {
            m_RiseRate = riseRate;
        }

        public override void Initialise()
        {
            base.Initialise();
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

            if (m_ObjReference != null)
                m_ObjReference.PosY += (m_RiseRate * (float)deltaTime.ElapsedGameTime.TotalSeconds);
        }
    }
}
