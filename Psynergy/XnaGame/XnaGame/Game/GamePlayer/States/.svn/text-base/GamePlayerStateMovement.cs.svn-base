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

using Psynergy.Camera;
using Psynergy.Menus;

namespace XnaGame
{
    class GamePlayerStateMovement : GamePlayerState
    {
        public GamePlayerStateMovement() : base()
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

            // Show idle animation if done
            if (player.ActivePawn != null)
                player.ActivePawn.ShowMovement();
        }

        public override void Update(GameTime deltaTime, GamePlayer player)
        {
            if (player.ActivePawn != null)
            {
                TutorialManager tutorials = TutorialManager.Instance;
                BoardSquare current = player.ActivePawn.CurrentSquare;

                // Show idle animation if done
                if (player.ActivePawn.AnimationPlayer != null)
                {
                    if (player.ActivePawn.AnimationPlayer.Done)
                        player.ActivePawn.ShowMovement();
                }

                // Set the next soldier spline position
                if (player.ActivePawn.Spline.IsEndReached(player.Position))
                {
                    // Empty the spline
                    player.ActivePawn.Spline.ClearSpline();

                    // Check if this next square is a question square or not
                    if (current != null)
                    {
                        if (current.Name == "IntXSquare")
                        {
                            // Check for any tutorials that should be shown
                            if (!tutorials.HasTutorialBeenShown(TutorialStage.DeclareXTutorial))
                            {
                                // Show start tutorial
                                tutorials.ShowTutorial(TutorialStage.DeclareXTutorial, Vector3.Zero, UIManager.TextureAlignment.eTopLeft);
                            }
                        }
                        else if (current.Name == "MainSquare")
                        {
                            if (!tutorials.HasTutorialBeenShown(TutorialStage.EnterMain))
                            {
                                // Show start tutorial
                                tutorials.ShowTutorial(TutorialStage.EnterMain, Vector3.Zero, UIManager.TextureAlignment.eTopLeft);
                            }
                        }
                        else if (current.GetType() == typeof(IfSquare))
                        {
                            if (!tutorials.HasTutorialBeenShown(TutorialStage.IfSquare))
                            {
                                // Show start tutorial
                                tutorials.ShowTutorial(TutorialStage.IfSquare, Vector3.Zero, UIManager.TextureAlignment.eTopLeft);
                            }
                        }
                        else if (current.GetType() == typeof(WhileSquare))
                        {
                            if (!tutorials.HasTutorialBeenShown(TutorialStage.WhileSquare))
                            {
                                // Show start tutorial
                                tutorials.ShowTutorial(TutorialStage.WhileSquare, Vector3.Zero, UIManager.TextureAlignment.eTopLeft);
                            }
                        }
                        else if (current.GetType() == typeof(SwitchSquare))
                        {
                            if (!tutorials.HasTutorialBeenShown(TutorialStage.SwitchSquare))
                            {
                                // Show start tutorial
                                tutorials.ShowTutorial(TutorialStage.SwitchSquare, Vector3.Zero, UIManager.TextureAlignment.eTopLeft);
                            }
                        }

                        if (current.QuestionSquare)
                        {
                            // If it is the first question and they are passing it, 
                            // fire a first question passed event and show the first question
                            // tutorial.
                            if (current.FirstQuestionSquare && !current.FirstQuestionPassed)
                            {
                                FirstQuestionPassedEvent firstQuestionEvent = new FirstQuestionPassedEvent();
                                firstQuestionEvent.Fire();

                                // Question tutorial
                                if (!tutorials.HasTutorialBeenShown(TutorialStage.Questions))
                                {
                                    // Show start tutorial
                                    tutorials.ShowTutorial(TutorialStage.Questions, Vector3.Zero, UIManager.TextureAlignment.eTopLeft);
                                }
                            }
                        }
                    }

                    if (player.CurrentDiceRoll > 0)
                    {
                        // Decrease the dice roll
                        player.CurrentDiceRoll--;

                        if (player.CurrentDiceRoll != 0)
                        {
                            player.Data = new BoardSquare.DecisionData();

                            if ((current.GetType() == typeof(IfSquare) || (current.GetType() == typeof(WhileSquare))))
                                player.Data = current.Decision(player.PreDecisionRoll);

                            // If not default route, switch to the jump state briefly
                            if (player.Data.route != BoardSquare.Route.eDefault)
                                player.ChangeState_Jump();
                            else
                            {
                                player.MoveToNextSquare();
                            }
                        }
                        else
                        {
                            // Check if this square is a question 
                            // ( if so they have landed on it and have a chance to go a bit further )
                            if (current.QuestionSquare)
                            {
                                // Change to the question state
                                player.ChangeState_Question();
                            }
                            else
                            {
                                player.ChangeState_EndOfTurn();
                            }
                        }
                    }
                    else if (player.CurrentDiceRoll < 0)
                    {
                        // Decrease the dice roll
                        player.CurrentDiceRoll++;

                        if (player.CurrentDiceRoll != 0)
                        {
                            //player.Data = new BoardSquare.DecisionData();

                            //if ((player.ActivePawn.CurrentSquare.GetType() == typeof(IfSquare) || (player.ActivePawn.CurrentSquare.GetType() == typeof(WhileSquare))))
                                //player.Data = player.ActivePawn.CurrentSquare.Decision(player.CurrentDiceRoll);

                            // Then it is at the end of the set spline/
                            // ( for now run the next square code again )
                            player.MoveToPreviousSquare();
                        }
                        else
                        {
                            player.ChangeState_EndOfTurn();
                        }
                    }
                    else
                    {
                        if (player.ActivePawn.CurrentSquare.GetType() != typeof(EndSquare))
                            player.ChangeState_EndOfTurn();
                        else
                            player.ChangeState_EndOfBoard();
                    }
                }

                // Show trail particles
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
