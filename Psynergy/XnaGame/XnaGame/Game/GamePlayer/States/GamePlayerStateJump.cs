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
    class GamePlayerStateJump : GamePlayerState
    {
        private float m_JumpTime = 2.0f;

        public GamePlayerStateJump() : base()
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

            m_JumpTime = 0.0f;

            if (player.ActivePawn != null)
            {
                m_JumpTime = player.ActivePawn.ShowJump();

                BoardSquare square = player.ActivePawn.CurrentSquare;

                if (square != null)
                {
                    if (square.GetType() == typeof(IfSquare))
                    {
                        UIManager.Instance.AddText3DObject("iftext", "Enter If Statement", (player.ActivePawn.transform.Position + new Vector3(0, +15, 0)), Color.OrangeRed, 1.5f);
                    }
                    else if (square.GetType() == typeof(WhileSquare))
                    {
                        UIManager.Instance.AddText3DObject("whiletext", "Enter While Loop", (player.ActivePawn.transform.Position + new Vector3(0, +15, 0)), Color.OrangeRed, 1.5f);
                    }
                }
            }
        }

        public override void Update(GameTime deltaTime, GamePlayer player)
        {
            // Switch back to the movement state after 2 seconds
            float elapsedTime = (float)deltaTime.ElapsedGameTime.TotalSeconds;

            if ( (m_JumpTime -= elapsedTime) <= 0.0f )
            {
                // If there is any movement left then switch to the movement state
                if (player.CurrentDiceRoll != 0)
                {
                    // Move to the next square and continue movement code as normal
                    player.MoveToNextSquare();
                    player.ChangeState_Movement();
                }
                else
                    player.ChangeState_Idle();
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
