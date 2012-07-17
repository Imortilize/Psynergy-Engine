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

namespace XnaGame
{
    class GamePlayerStateEndOfTurn : GamePlayerState
    {
        private bool m_End = false;

        public GamePlayerStateEndOfTurn() : base()
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

            player.CurrentDiceRoll = 0;
            player.PreDecisionRoll = 0;
            player.OriginalDiceRoll = 0;

            if (player.ActivePawn != null)
                player.ActivePawn.EndOfTurn();

            // For now end straight away
            m_End = false;
        }

        public override void Update(GameTime deltaTime, GamePlayer player)
        {
            if (!m_End)
            {
                // Change to idle state
                player.ChangeState_Idle();

                // Run next players turn
                PlayerManager.Instance.NextPlayer();

                // End
                m_End = true;
            }

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
