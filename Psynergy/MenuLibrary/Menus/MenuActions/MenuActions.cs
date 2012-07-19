using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/* Main Library */
using Psynergy;

/* Graphics library */
using Psynergy.Graphics;

/* Input Library */
using Psynergy.Input;

namespace Psynergy.Menus
{
    class MenuActionPlayGame : MenuAction, IRegister<MenuActionPlayGame>
    {
        public MenuActionPlayGame() 
            : base()
        {
        }

        public override void RunAction()
        {
            base.RunAction();

            RenderManager.Instance.GameStateManager.ChangeState("Game");
        }
    }

    class MenuActionOptions : MenuAction, IRegister<MenuActionOptions>
    {
        public MenuActionOptions()
            : base()
        {
        }

        public override void RunAction()
        {
            base.RunAction();

            RenderManager.Instance.GameStateManager.ChangeState("Game");
        }
    }

    class MenuActionExitGame : MenuAction, IRegister<MenuActionExitGame>
    {
        public MenuActionExitGame() 
            : base()
        {
        }

        public override void RunAction()
        {
            base.RunAction();

            InputManager.Instance.ExitApplication = true;
        }
    }

    class MenuActionReturnToGame : MenuAction, IRegister<MenuActionReturnToGame>
    {
        public MenuActionReturnToGame() 
            : base()
        {
        }

        public override void RunAction()
        {
            base.RunAction();

            MenuManager.Instance.CloseMenu();
            //InputManager.Instance.UnPauseGame();
        }
    }

    class MenuActionReturnToMainMenu : MenuAction, IRegister<MenuActionReturnToMainMenu>
    {
        public MenuActionReturnToMainMenu() : base()
        {
        }

        public override void RunAction()
        {
            base.RunAction();

            // Make sure the game is unpaused
            InputManager.Instance.UnPauseGame();

            // Change to the main menu state
            RenderManager.Instance.GameStateManager.ChangeState("MainMenu");
        }
    }

    class MenuActionRunTestState : MenuAction, IRegister<MenuActionRunTestState>
    {
        public MenuActionRunTestState() : base()
        {
        }

        public override void RunAction()
        {
            base.RunAction();

            RenderManager.Instance.GameStateManager.ChangeState("Game");
        }
    }
}
