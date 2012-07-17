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
    public class MenuTransition : GameObject
    {
        #region Factory Property setting
        protected override void ClassProperties(Factory factory)
        {
            base.ClassProperties(factory);
        }
        #endregion

        protected MenuBackground m_MenuBackground = null;
        protected List<MenuOption> m_MenuOptions = new List<MenuOption>();

        // This specifies whether to restart the transition or not ( can be useful when going back in a menu chain and we don't want to reset the menu background )
        protected bool m_RestartBackgroundTransition = true;
        protected bool m_RestartOptionsTransition = true;     

        public MenuTransition()
        {
        }

        public override void Reset()
        {
            base.Reset();

            if (m_RestartBackgroundTransition)
            {
                if (m_MenuBackground != null)
                    ResetBackground();
            }

            if (m_RestartOptionsTransition)
            {
                if (m_MenuOptions.Count > 0)
                    ResetOptions();
            }
        }

        protected virtual void ResetBackground()
        {
        }

        protected virtual void ResetOptions()
        {
        }

        public virtual void OnEnter()
        {
        }

        public virtual void OnClose()
        {
        }

        public virtual bool UpdateMenuBackgroundOpenTransition(GameTime deltaTime)
        {
            return true;
        }

        public virtual bool UpdateMenuOptionOpenTransitions(GameTime deltaTime)
        {
            return true;
        }

        public virtual bool UpdateMenuBackgroundCloseTransition(GameTime deltaTime)
        {
            return true;
        }

        public virtual bool UpdateMenuOptionCloseTransitions(GameTime deltaTime)
        {
            return true;
        }

        public void AddMenuOption(MenuOption menuOption)
        {
            Debug.Assert(menuOption != null, "Menu option cannot be added if null!");

            // Add menu option
            if ( menuOption != null )
                m_MenuOptions.Add(menuOption);
        }

        public void AddMenuBackground(MenuBackground menuBackground)
        {
            Debug.Assert(menuBackground != null, "Menu background cannot be added if null!");

            // Add menu option
            if (menuBackground != null)
                m_MenuBackground = menuBackground;
        }

        #region Property Set / Gets
        public bool RestartBackgroundTransition { get { return m_RestartBackgroundTransition; } set { m_RestartBackgroundTransition = value; } }
        public bool RestartOptionsTransition { get { return m_RestartOptionsTransition; } set { m_RestartOptionsTransition = value; } }
        #endregion
    }
}
