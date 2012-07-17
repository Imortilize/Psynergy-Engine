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

/* AI Library */
using Psynergy.AI;

namespace XnaGame
{
    class GamePlayerState : State<GamePlayer>
    {
        public GamePlayerState() : base()
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
        }

        public override void Update(GameTime deltaTime, GamePlayer player)
        {
            // MAKE sure this is after changing state
            base.Update(deltaTime, player);

            // If closed then return
            if (m_Closed)
                return;
        }

        protected override bool CloseChecks()
        {
            return true;
        }

        public override void OnExit(GamePlayer player)
        {
            base.OnExit(player);

            // Exit the state...
        }
    }
}
