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
    class GamePlayerStateDeselected : GamePlayerState
    {
        public GamePlayerStateDeselected() : base()
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

            // Set selected
            player.Selected = false;

            if (player.Dice != null)
                player.Dice.Disable();


            // UnFocus the player indicator
            player.UnFocusUIInterfaceObject(GamePlayer.UIInterfaceObject.PlayerIndicator);

            if (player.PawnsAtEnd > 0)
                player.UnFocusUIInterfaceObject(GamePlayer.UIInterfaceObject.PlayerIndicator2);

            if (player.PawnsAtEnd > 1)
                player.UnFocusUIInterfaceObject(GamePlayer.UIInterfaceObject.PlayerIndicator3);
        }

        public override void Update(GameTime deltaTime, GamePlayer player)
        {
            // Change back to idle state
            player.ChangeState_Idle();

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
