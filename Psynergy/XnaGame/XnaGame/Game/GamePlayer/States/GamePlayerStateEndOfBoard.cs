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
    class GamePlayerStateEndOfBoard : GamePlayerState
    {
        public GamePlayerStateEndOfBoard() : base()
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

            // Zoom camera out
            player.ZoomOut();

            // Not celebrated yet
            player.ActivePawn.ShowReachedEnd();
        }

        public override void Update(GameTime deltaTime, GamePlayer player)
        {
            if (player.ActivePawn != null)
            {
                // Show idle animation if done
                if (player.ActivePawn.AnimationPlayer.Done)
                {
                    if (player.PawnsAtEnd == 0)
                        player.ShowFirstPawnAtEnd();
                    else if (player.PawnsAtEnd == 1)
                        player.ShowSecondPawnAtEnd();

                    // Increment number of pawns reached end for this player
                    player.PawnsAtEnd += 1;

                    if (player.PawnsAtEnd == player.Pawns.Count)
                    {
                        // Player has won the game so fire a player won event with this players index
                        // attached to it
                        PlayerWonEvent wonEvent = new PlayerWonEvent(player.Index);
                        wonEvent.Fire();
                    }
                    else
                    {
                        // Otherwise end turn
                        player.ChangeState_EndOfTurn();
                    }
                }
                else
                    player.ActivePawn.ShowTrail();
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
