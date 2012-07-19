using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;

/* Main Library */
using Psynergy;

/* AI Library */
using Psynergy.AI;

namespace Psynergy.Menus
{
    public class MainMenuState : State<GameObject>, IRegister<MainMenuState>
    {
        private MenuManager.Menus m_MenuToCheck = MenuManager.Menus.NumPlayersMenu;

        public MainMenuState() : base()
        {
        }

        public override void Initialise()
        {
        }

        public override void Load()
        {
        }

        public override void Reset()
        {
        }

        public override void OnEnter(GameObject objectRef)
        {
            base.OnEnter(objectRef);

            // Show the main menu
            MenuManager.Instance.ShowMenu(MenuManager.Menus.MainMenu, true);
        }

        public override void Update(GameTime deltaTime, GameObject objectRef)
        {
            base.Update(deltaTime, objectRef);
        }

        public override void Render(GameTime deltaTime, GameObject objectRef)
        {
            base.Render(deltaTime, objectRef);
        }

        protected override bool CloseChecks()
        {
            return MenuManager.Instance.IsMenuClosed(m_MenuToCheck);
        }

        public override void OnExit(GameObject objectRef)
        {
            base.OnExit(objectRef);

            // Close the main menu
            MenuManager.Instance.CloseMenu();
        }

        public override void OnFinish()
        {
            base.OnFinish();
        }
    }
}
