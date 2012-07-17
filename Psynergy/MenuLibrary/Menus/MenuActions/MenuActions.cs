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
    class MenuActionPlayGame : MenuAction
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

    class MenuActionOptions : MenuAction
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

    class MenuActionExitGame : MenuAction
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

    class MenuActionReturnToGame : MenuAction
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

    class MenuActionReturnToMainMenu : MenuAction
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

    class MenuActionRunTestState : MenuAction
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
