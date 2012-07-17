using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/* Main Library */
using Psynergy;
using Psynergy.Graphics;
using Psynergy.Input;
using Psynergy.Menus;
using Psynergy.Sound;

namespace XnaGame
{
    /* Game Specific */
    public class MenuActionPlayGameOnePlayer : MenuAction
    {
        public MenuActionPlayGameOnePlayer() : base()
        {
        }

        public override void RunAction()
        {
            base.RunAction();

            // Set player manager to accept one player
            PlayerManager.Instance.NumberPlayersToUse = 2;

            // Get the second player
            Player player1 = PlayerManager.Instance.GetPlayer(PlayerIndex.One);
            Player player2 = PlayerManager.Instance.GetPlayer(PlayerIndex.Two);

            //if (player1 != null)
                //player1.Control = Player.PlayerControl.AI;

            if (player2 != null)
                player2.Control = Player.PlayerControl.AI;

            // Tell Render Manager to switch to game state
            RenderManager.Instance.GameStateManager.ChangeState("Game");
        }
    }

    public class MenuActionPlayGameTwoPlayer : MenuAction
    {
        public MenuActionPlayGameTwoPlayer() : base()
        {
        }

        public override void RunAction()
        {
            base.RunAction();

            // Set player manager to accept one player
            PlayerManager.Instance.NumberPlayersToUse = 2;

            // Get the second player
            Player player = PlayerManager.Instance.GetPlayer(PlayerIndex.Two);

            if (player != null)
                player.Control = Player.PlayerControl.Human;

            // Tell Render Manager to switch to game state
            RenderManager.Instance.GameStateManager.ChangeState("Game");
        }
    }

    public class MenuActionPreviousPawn : MenuAction
    {
        public MenuActionPreviousPawn() : base()
        {
        }

        public override void RunAction()
        {
            base.RunAction();

            GamePlayer activePlayer = (PlayerManager.Instance.ActivePlayer as GamePlayer);

            if (activePlayer != null)
                activePlayer.FocusNextPawn();
        }
    }

    public class MenuActionNextPawn : MenuAction
    {
        public MenuActionNextPawn() : base()
        {
        }

        public override void RunAction()
        {
            base.RunAction();

            GamePlayer activePlayer = (PlayerManager.Instance.ActivePlayer as GamePlayer);

            if (activePlayer != null)
                activePlayer.FocusNextPawn();
        }
    }

    public class MenuActionReplayGame : MenuAction
    {
        public MenuActionReplayGame()
            : base()
        {
        }

        public override void RunAction()
        {
            base.RunAction();

            RenderManager.Instance.GameStateManager.ChangeState("GameState");
        }
    }

    public class MenuActionToggleFullScreen : MenuAction
    {
        public MenuActionToggleFullScreen() : base()
        {
        }

        public override void RunAction()
        {
            base.RunAction();

            GraphicsDeviceManager graphics = RenderManager.Instance.GraphicsDeviceManager;
            GraphicsAdapter adapter = RenderManager.Instance.GraphicsDeviceManager.GraphicsDevice.Adapter;

            if (!graphics.IsFullScreen)
            {
                DisplayMode display = adapter.CurrentDisplayMode;

                graphics.PreferredBackBufferWidth = display.Width;
                graphics.PreferredBackBufferHeight = display.Height;
            }
            else
            {
                //if (adapter.IsWideScreen)
                //{
                    graphics.PreferredBackBufferWidth = 1280;
                    graphics.PreferredBackBufferHeight = 720;
                //}
                //else
                //{
                    //graphics.PreferredBackBufferWidth = 1024;
                    //graphics.PreferredBackBufferHeight = 768;
                //}
            }

            graphics.ApplyChanges();
            graphics.ToggleFullScreen();
        }
    }

    public class MenuActionToggleMute : MenuAction
    {
        public MenuActionToggleMute() : base()
        {
        }

        public override void RunAction()
        {
            base.RunAction();

            SoundManager soundManager = SoundManager.Instance;

            if ( !soundManager.IsMuted() )
                soundManager.Mute();
            else
                soundManager.UnMute();
        }
    }

    public class MenuActionToggleGraphics : MenuAction
    {
        public MenuActionToggleGraphics() : base()
        {
        }

        public override void RunAction()
        {
            base.RunAction();

            RenderManager renderManager = RenderManager.Instance;

            // Toggle graphics
            renderManager.ToggleGraphicsSettings();
        }
    }

    public class MenuActionRollDice : MenuAction
    {
        public MenuActionRollDice() : base()
        {
        }

        public override void RunAction()
        {
            base.RunAction();

            Player player = PlayerManager.Instance.ActivePlayer;

            if (player != null)
            {
                GamePlayer gamePlayer = (player as GamePlayer);

                if (gamePlayer != null)
                {
                    // Begin rolling the dice
                    gamePlayer.BeginRollDice();
                }
            }
        }
    }
      
    public class MenuActionQuestionAnswer : MenuAction
    {
        private Question m_Question = null;
        private int m_Index = 0;
        private int m_Answer = 0;

        public MenuActionQuestionAnswer(Question question, int index, int answer)
        {
            m_Question = question;
            m_Index = index;
            m_Answer = answer;
        }

        public override void RunAction()
        {
            base.RunAction();

            if (m_Question != null)
            {
                if (m_Index == m_Answer)
                {
                    // Answer is correct
                    m_Question.SetAnswer(true);
                }
                else
                {
                    // Answer is incorrect
                    m_Question.SetAnswer(false);
                }
            }
        }
    }
}
