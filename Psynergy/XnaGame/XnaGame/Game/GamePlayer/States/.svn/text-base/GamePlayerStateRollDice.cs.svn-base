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
    class GamePlayerStateRollDice : GamePlayerState
    {
        public GamePlayerStateRollDice() : base()
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

            // Roll the dice
            player.RollDice();

            // Zoom camera out
            //player.ZoomOut();
        }

        public override void Update(GameTime deltaTime, GamePlayer player)
        {
            if (player.PreDecisionRoll == 0)
            {
                if (player.Dice != null)
                {
                    // If the dice has stopped rolling
                    if (player.Dice.IsStationary())
                    {
                        // Calculate the dice roll number and generate roll data
                        // Get the dice roll and then take the square stats into account to determine the final 
                        // dice roll value
                        player.PreDecisionRoll = player.Dice.FindResult();

                        // Get the square decision data
                        player.GenerateDecisionData();

                        // Focus the camera back onto the player
                        PlayerManager.Instance.SetCameraFocus(player.Index);

                        // Zoom out
                        player.ZoomOut();

                        // Move to the zoom camera state
                        //player.CameraZoom = true;

                        // Remove player ui objects ( for now all of them here )
                        //player.RemoveUIInterfaceObjects();

                        // Change to pre movement state
                        player.ChangeState_PreMovement();
                    }

                    // Show selected effect
                    player.ActivePawn.ShowSelected();
                }
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
