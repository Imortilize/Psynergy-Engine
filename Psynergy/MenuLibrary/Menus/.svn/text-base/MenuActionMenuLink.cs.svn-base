using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Psynergy.Menus
{
    class MenuActionMenuLink : MenuAction
    {
        protected MenuManager.Menus m_MenuToShow = MenuManager.Menus.Error;
        protected bool m_ResetMenuBackgroundTransition = true;
        protected bool m_ResetMenuOptionsTransition = true;

        public MenuActionMenuLink()
        {
        }

        public void SetMenuLink(MenuManager.Menus menu)
        {
            Debug.Assert(menu >= 0, "[WARNING] - menu link must not be null!");

            if (menu >= 0)
                m_MenuToShow = menu;
        }

        public override void RunAction()
        {
            base.RunAction();

            //Debug.Assert(m_MenuToShow >= 0, "[WARNING] - Menu to return to from " + Name + " is invalid!");

            // Show the menu that should have been assigned at load up
            if (m_MenuToShow >= 0)
                MenuManager.Instance.ShowMenu(m_MenuToShow, m_ResetMenuBackgroundTransition, m_ResetMenuOptionsTransition);
        }

        #region Property Set / Gets
        public MenuManager.Menus MenuToShow { get { return m_MenuToShow; } set { if (m_MenuToShow < 0) m_MenuToShow = value; } }
        public bool ResetMenuBackgroundTransition { get { return m_ResetMenuBackgroundTransition; } set { m_ResetMenuBackgroundTransition = value; } }
        public bool ResetMenuOptionsTransition { get { return m_ResetMenuOptionsTransition; } set { m_ResetMenuOptionsTransition = value; } }
        #endregion
    }
}
