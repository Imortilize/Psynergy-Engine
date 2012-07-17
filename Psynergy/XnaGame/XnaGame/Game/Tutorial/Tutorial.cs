using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using Psynergy;
using Psynergy.Menus;
using Psynergy.Graphics;
using Psynergy.Input;

namespace XnaGame
{
    public class Tutorial : UIObject
    {
        private bool        m_Shown = false;
        private bool        m_Close = false;

        public Tutorial(String name) : base(name)
        {
        }

        public Tutorial(String name, String textureName, float fadeRate) : base(name, textureName, Vector3.Zero)
        {
            UIObjectEffectFadeInFadeOut effect = new UIObjectEffectFadeInFadeOut(fadeRate);

            // Set the effect
            effect.SetUIObject(this);
        }

        public override void Initialise()
        {
            base.Initialise();

            // So they render above other UI objects
            m_RenderDepth = 0.0f;
        }

        public override void Load()
        {
            base.Load();
        }

        public override void Update(GameTime deltaTime)
        {
            base.Update(deltaTime);

            if (m_ActiveState == ActiveState.eIdle)
            {
                if (InputManager.Instance.IsAnyInput(PlayerIndex.One))
                    m_Close = true;
            }
        }

        public bool Shown { get { return m_Shown; } set { m_Shown = value; } }
        public bool Close { get { return m_Close; } set { m_Close = value; } } 
    }
}
