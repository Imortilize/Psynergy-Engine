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
    class GamePlayerStateQuestion : GamePlayerState
    {
        private const int m_BonusMoveSquares = 2;
        private Random m_Random = new Random();

        public GamePlayerStateQuestion() : base()
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

            // If this is an actual player show the question
            if (player.Control == Player.PlayerControl.Human)
            {
                // Show a question from the pool of questions
                QuestionManager.Instance.ShowQuestion();
            }
        }

        public override void Update(GameTime deltaTime, GamePlayer player)
        {
            QuestionManager questions = QuestionManager.Instance;

            Player.PlayerControl control = player.Control;

            // Question has been answered
            if (questions.IsQuestionAnswered() || (control == Player.PlayerControl.AI))
            {
                bool answer = false;
                
                if ( control == Player.PlayerControl.Human )
                    answer = questions.QuestionAnswer();
                else if (control == Player.PlayerControl.AI)
                {
                    int number = m_Random.Next(0, 10);

                    if (number < 5)
                        answer = true;
                    else
                        answer = false;
                }

                if (answer == true)
                {
                    UIManager.Instance.AddText3DObject("correcttext", "Correct", (player.ActivePawn.transform.Position + new Vector3(0, +15, 0)), Color.ForestGreen, 1.5f);

                    // Set player to move two spaces forward
                    player.Data = new BoardSquare.DecisionData(m_BonusMoveSquares);
                }
                else
                {
                    UIManager.Instance.AddText3DObject("incorrecttext", "Incorrect", (player.ActivePawn.transform.Position + new Vector3(0, +15, 0)), Color.OrangeRed, 1.5f);

                    // Set player to move to spaces backwards
                    player.Data = new BoardSquare.DecisionData(m_BonusMoveSquares);
                }

                // Remove question
                questions.Remove();

                // Change to end of turn state straight away for now
                player.ChangeState_PreMovement();
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
