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
    public class UIObjectTimed : UIObject
    {
        private UIObject m_UIObject = null;
        private float m_Time = 0.0f;

        public UIObjectTimed() : base()
        {
        }

        public UIObjectTimed(UIObject UIObject, float time)
        {
            m_UIObject = UIObject;
            m_Time = time;
        }

        public override void Initialise()
        {
            base.Initialise();
        }

        public override void Reset()
        {
            base.Reset();

            m_Time = 0.0f;
        }

        public override void Load()
        {
            base.Load();
        }

        public override void UnLoad()
        {
            base.UnLoad();
        }

        public override void Update(GameTime deltaTime)
        {
            base.Update(deltaTime);

            // Time increment
            float interval = (float)deltaTime.ElapsedGameTime.TotalSeconds;

            if ((m_Time -= interval) <= 0.0f)
            {
                // Timer has run out so start that the uiobject should be removed
                m_RemoveUIObject = true;
            }
        }

        public override void Render(GameTime deltaTime)
        {
            // This object doesn't render itself, it renders the attached UIObject
            if (m_UIObject != null)
                m_UIObject.Render(deltaTime);
        }
    }
}
