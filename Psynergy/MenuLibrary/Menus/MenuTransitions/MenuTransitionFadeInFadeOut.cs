using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

/* Main Library */
using Psynergy;

namespace Psynergy.Menus
{
    class MenuTransitionFadeInFadeOut : MenuTransition
    {
        private float m_BackgroundStartFade = 1.0f;
        private float m_BackgroundEndFade = 1.0f;
        private float m_BackgroundFadeRate = 1.0f;
        private float m_OptionStartFade = 0.0f;
        private float m_OptionEndFade = 1.0f;
        private float m_OptionFadeRate = 1.0f;
        private float m_CurrentOptionFadeDelay = 0.0f;
        private float m_OptionFadeDelay = 0.0f;
        private int m_OptionsActive = 0;

        #region Factory Property setting
        protected override void ClassProperties(Factory factory)
        {
            factory.RegisterFloat("backgroundstartfade", "BackgroundStartFade");
            factory.RegisterFloat("backgroundendfade", "BackgroundEndFade");
            factory.RegisterFloat("backgroundfaderate", "BackgroundFadeRate");
            factory.RegisterFloat("optionstartfade", "OptionStartFade");
            factory.RegisterFloat("optionendfade", "OptionEndFade");
            factory.RegisterFloat("optionfaderate", "OptionFadeRate");
            factory.RegisterFloat("optionfadedelay", "OptionFadeDelay");

            base.ClassProperties(factory);
        }
        #endregion

        public MenuTransitionFadeInFadeOut() : base()
        {
        }

        public override void Initialise()
        {
            base.Initialise();
        }

        public override void Reset()
        {
            base.Reset();
        }

        protected override void ResetBackground()
        {
            m_MenuBackground.Opacity = m_BackgroundStartFade;
        }

        protected override void ResetOptions()
        {
            foreach (MenuOption menuOption in m_MenuOptions)
                menuOption.Opacity = m_OptionStartFade;
        }

        public override void OnEnter()
        {
            // Reset the option fade delay
            m_CurrentOptionFadeDelay = m_OptionFadeDelay;
            m_OptionsActive = 0;
        }

        public override void OnClose()
        {
            // Reset the option fade delay
            m_CurrentOptionFadeDelay = m_OptionFadeDelay;
            m_OptionsActive = 0;
        }

        public override bool UpdateMenuOptionOpenTransitions(GameTime deltaTime)
        {
            bool toRet = true;

            // Check if there are any more options to activate
            toRet = UpdateActivateMenuOptions(deltaTime);

            for (int i = 0; i < m_MenuOptions.Count; i++)
            {
                if (i < m_OptionsActive)
                {
                    if (m_MenuOptions[i].Opacity < m_OptionEndFade)
                    {
                        float newOpacity = m_MenuOptions[i].Opacity + (m_OptionFadeRate * (float)deltaTime.ElapsedGameTime.TotalSeconds);

                        // Check the new opacity is below the final opacity wanted
                        if (newOpacity > m_OptionEndFade)
                            newOpacity = m_OptionEndFade;

                        // Increase the opacity
                        m_MenuOptions[i].Opacity = newOpacity;

                        if (newOpacity != m_OptionEndFade)
                            toRet = false;
                    }
                }
            }

            return toRet;
        }

        public override bool UpdateMenuOptionCloseTransitions(GameTime deltaTime)
        {
            bool toRet = true;

            // Check if there are any more options to activate
            toRet = UpdateActivateMenuOptions(deltaTime);

            for (int i = 0; i < m_MenuOptions.Count; i++)
            {
                if (i < m_OptionsActive)
                {
                    if (m_MenuOptions[i].Opacity > m_OptionStartFade)
                    {
                        float newOpacity = m_MenuOptions[i].Opacity - (m_OptionFadeRate * (float)deltaTime.ElapsedGameTime.TotalSeconds);

                        // Check the new opacity is below the final opacity wanted
                        if (newOpacity < m_OptionStartFade)
                            newOpacity = m_OptionStartFade;

                        // Increase the opacity
                        m_MenuOptions[i].Opacity = newOpacity;

                        if (newOpacity != m_OptionStartFade)
                            toRet = false;
                    }
                }
            }

            return toRet;
        }

        private bool UpdateActivateMenuOptions(GameTime deltaTime)
        {
            bool toRet = true;

            if (m_OptionsActive < m_MenuOptions.Count)
            {
                toRet = false;

                m_CurrentOptionFadeDelay += (float)deltaTime.ElapsedGameTime.TotalSeconds;

                // Check if there are any more options to activate
                if (m_CurrentOptionFadeDelay >= m_OptionFadeDelay)
                {
                    m_OptionsActive++;

                    // Reset the option fade delay
                    m_CurrentOptionFadeDelay = 0.0f;

                    if (m_OptionsActive >= m_MenuOptions.Count)
                        toRet = true;
                }
            }

            return toRet;
        }

        public override bool UpdateMenuBackgroundOpenTransition(GameTime deltaTime)
        {
            bool toRet = true;

            if (m_MenuBackground != null)
            {
                if (m_MenuBackground.Opacity < m_BackgroundEndFade)
                {
                    float newOpacity = (m_MenuBackground.Opacity + (m_BackgroundFadeRate * (float)deltaTime.ElapsedGameTime.TotalSeconds));

                    if (newOpacity > m_BackgroundEndFade)
                        newOpacity = m_BackgroundEndFade;

                    m_MenuBackground.Opacity = newOpacity;

                    if (newOpacity != m_BackgroundEndFade)
                        toRet = false;
                }
            }

            return toRet;
        }

        public override bool UpdateMenuBackgroundCloseTransition(GameTime deltaTime)
        {
            bool toRet = true;

            if (m_MenuBackground != null)
            {
                if (m_MenuBackground.Opacity > m_BackgroundStartFade)
                {
                    float newOpacity = (m_MenuBackground.Opacity - (m_BackgroundFadeRate * (float)deltaTime.ElapsedGameTime.TotalSeconds));

                    if (newOpacity < m_BackgroundStartFade)
                        newOpacity = m_BackgroundStartFade;

                    m_MenuBackground.Opacity = newOpacity;

                    if (newOpacity != m_BackgroundStartFade)
                        toRet = false;
                }
            }


            return toRet;
        }

        #region Property Set / Gets
        public float BackgroundStartFade { get { return m_BackgroundStartFade; } set { m_BackgroundStartFade = value; } }
        public float BackgroundEndFade { get { return m_BackgroundEndFade; } set { m_BackgroundEndFade = value; } }
        public float BackgroundFadeRate { get { return m_BackgroundFadeRate; } set { m_BackgroundFadeRate = value; } }
        public float OptionStartFade { get { return m_OptionStartFade; } set { m_OptionStartFade = value; } }
        public float OptionEndFade { get { return m_OptionEndFade; } set { m_OptionEndFade = value; } }
        public float OptionFadeRate { get { return m_OptionFadeRate; } set { m_OptionFadeRate = value; } }
        public float OptionFadeDelay { get { return m_OptionFadeDelay; } set { m_OptionFadeDelay = value; } }
        #endregion
    }
}
