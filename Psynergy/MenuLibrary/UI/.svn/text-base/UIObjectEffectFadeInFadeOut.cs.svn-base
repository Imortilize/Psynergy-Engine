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
    public class UIObjectEffectFadeInFadeOut : UIObjectEffect
    {
        private float m_FadeInRate = 0.0f;
        private float m_FadeOutRate = 0.0f;

        public UIObjectEffectFadeInFadeOut( float fadeRate ) : base()
        {
            m_FadeInRate = fadeRate;
            m_FadeOutRate = fadeRate;
        }

        public UIObjectEffectFadeInFadeOut(float fadeInRate, float fadeOutRate) : base()
        {
            m_FadeInRate = fadeInRate;
            m_FadeOutRate = fadeOutRate;
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
        }

        public override void OnEnter()
        {
            if (m_ObjReference != null)
                m_ObjReference.Opacity = 0.0f;
        }

        public override void OnExit()
        {
            if (m_ObjReference != null)
                m_ObjReference.Opacity = 1.0f;
        }

        protected override void OnIn(GameTime deltaTime)
        {
            float opacity = m_ObjReference.Opacity;

            if (opacity < 1.0f)
            {
                float newOpacity = opacity + (m_FadeInRate * (float)deltaTime.ElapsedGameTime.TotalSeconds);

                // Check the new opacity is below the final opacity wanted
                if (newOpacity > 1.0f)
                    newOpacity = 1.0f;

                // Increase the opacity
                m_ObjReference.Opacity = newOpacity;
            }

            if ( m_ObjReference.Opacity >= 1.0f )
                m_ObjReference.SetActiveState(UIObject.ActiveState.eIdle);
        }

        protected override void OnIdle(GameTime deltaTime)
        {
        }

        protected override void OnOut(GameTime deltaTime)
        {
            float opacity = m_ObjReference.Opacity;

            if (opacity > 0.0f)
            {
                float newOpacity = opacity - (m_FadeOutRate * (float)deltaTime.ElapsedGameTime.TotalSeconds);

                // Check the new opacity is below the final opacity wanted
                if (newOpacity < 0.0f)
                    newOpacity = 0.0f;

                // Increase the opacity
                m_ObjReference.Opacity = newOpacity;
            }

            if (m_ObjReference.Opacity <= 0.0f)
                m_ObjReference.SetActiveState(UIObject.ActiveState.eRemove);
        }

        protected override void OnFocus(GameTime deltaTime)
        {
            float opacity = m_ObjReference.Opacity;

            if (opacity < 1.0f)
            {
                float newOpacity = opacity + (m_FadeInRate * (float)deltaTime.ElapsedGameTime.TotalSeconds);

                // Check the new opacity is below the final opacity wanted
                if (newOpacity > 1.0f)
                    newOpacity = 1.0f;

                // Increase the opacity
                m_ObjReference.Opacity = newOpacity;
            }

            if (m_ObjReference.Opacity >= 1.0f)
                m_ObjReference.SetActiveState(UIObject.ActiveState.eIdle);
        }

        protected override void OnUnFocus(GameTime deltaTime)
        {
            float opacity = m_ObjReference.Opacity;

            if (opacity > 0.0f)
            {
                float newOpacity = opacity - (m_FadeOutRate * (float)deltaTime.ElapsedGameTime.TotalSeconds);

                // Check the new opacity is below the final opacity wanted
                if (newOpacity < 0.3f)
                    newOpacity = 0.3f;

                // Increase the opacity
                m_ObjReference.Opacity = newOpacity;
            }
            else
                m_ObjReference.Opacity = 0.3f;

            if (m_ObjReference.Opacity <= 0.3f)
                m_ObjReference.SetActiveState(UIObject.ActiveState.eIdle);
        }
    }
}
