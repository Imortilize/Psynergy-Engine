using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Psynergy.Menus;

namespace XnaGame
{
    class GamePlayerStateWon : GamePlayerState
    {
        private float m_WaitBeforeEndMenuTime = 10.0f;
        private float m_CurrentWaitBeforeEndMenuTime = 0.0f;

        public GamePlayerStateWon() : base()
        {
        }

        public override void Initialise()
        {
            // Initialise the state...
        }

        public override void Load()
        {
            // Load the state...
        }

        public override void UnLoad()
        {
            // Unload the state...
        }

        public override void Reset()
        {
            // Reset the state...
        }

        public override void OnEnter(GamePlayer player)
        {
            base.OnEnter(player);

            foreach (GamePawn pawn in player.Pawns)
                pawn.ShowWon();

            m_CurrentWaitBeforeEndMenuTime = m_WaitBeforeEndMenuTime;
        }

        public override void Update(GameTime deltaTime, GamePlayer player)
        {
            foreach (GamePawn pawn in player.Pawns)
            {
                if (pawn.AnimationPlayer.Done)
                    pawn.ShowWon();
            }

            if (m_CurrentWaitBeforeEndMenuTime > 0.0f)
            {
                if ((m_CurrentWaitBeforeEndMenuTime -= (float)deltaTime.ElapsedGameTime.TotalSeconds) <= 0.0f)
                {
                    if (player.Index == PlayerIndex.One)
                    {
                        // Show final menu
                        MenuManager.Instance.ShowMenu(MenuManager.Menus.EndGameMenuPlayer1);
                    }
                    else if (player.Index == PlayerIndex.Two)
                    {
                        // Show final menu
                        MenuManager.Instance.ShowMenu(MenuManager.Menus.EndGameMenuPlayer2);
                    }
                }
            }

            foreach (GamePawn pawn in player.Pawns)
                pawn.ShowTrail();

            // MAKE sure this is after changing state
            base.Update(deltaTime, player);
        }

        public override void OnExit(GamePlayer player)
        {
            base.OnExit(player);

            // Exit the state...
        }
    }
}
