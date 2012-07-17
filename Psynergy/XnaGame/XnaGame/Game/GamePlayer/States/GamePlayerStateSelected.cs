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


using Psynergy.Graphics;
using Psynergy.Menus;
using Psynergy.Camera;

namespace XnaGame
{
    class GamePlayerStateSelected : GamePlayerState
    {
        public GamePlayerStateSelected() : base()
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

            // Show the player texture to say it is a new turn
            float width = RenderManager.Instance.GraphicsDevice.Viewport.Width;
            float height = RenderManager.Instance.GraphicsDevice.Viewport.Height;

            float xPos = (width * 0.5f);
            float yPos = (height * 0.25f);

            float border = 15.0f;

            // Select first pawn in collection by default
            if (player.Pawns.Count > 0)
            {
                int pawnsLeft = (player.Pawns.Count - player.PawnsAtEnd);
                int index = 0;

                if (player.Control != Player.PlayerControl.AI)
                {
                    // If more then one game pawn being used ( TODO )
                    if (pawnsLeft > 1)
                    {
                        // Add game pawn selectors ( Minimum of one
                        String gamePawn1 = player.GetUIInterfaceObjectName(GamePlayer.UIInterfaceObject.Pawn1);
                        UIManager.Instance.AddUIObject(gamePawn1, new Vector3(border, (height * 0.5f), 0.0f), UIManager.TextureAlignment.eCenterLeft);

                        if (player.Pawns.Count > 1)
                        {
                            String gamePawn2 = player.GetUIInterfaceObjectName(GamePlayer.UIInterfaceObject.Pawn2);
                            UIManager.Instance.AddUIObject(gamePawn2, new Vector3((width - border), (height * 0.5f), 0.0f), UIManager.TextureAlignment.eCenterRight);
                        }
                    }

                    // Bring up roll dice image
                    String rollDice = player.GetUIInterfaceObjectName(GamePlayer.UIInterfaceObject.RollDice);
                    UIManager.Instance.AddUIObject(rollDice, new Vector3((width * 0.5f), height, 0.0f), UIManager.TextureAlignment.eCenterBottom);
                }

                // Get the index of the pawn last selected
                if (player.ActivePawn != null)
                {
                    if ((player.ActivePawn.CurrentSquare != null) && (player.ActivePawn.CurrentSquare.GetType() != typeof(EndSquare)))
                        index = player.Pawns.IndexOf(player.ActivePawn);
                    else
                    {
                        foreach (GamePawn pawn in player.Pawns)
                        {
                            // Find the next pawn that isn't at the end square
                            if (pawn.CurrentSquare != null)
                            {
                                if (pawn.CurrentSquare.GetType() != typeof(EndSquare))
                                    index = player.Pawns.IndexOf(pawn);
                            }
                        }
                    }
                }

                // Set pawn index
                player.SetActivePawn(index);
            }
            else
                player.SetActivePawn(-1);

            // Set camera focus
            PlayerManager.Instance.SetCameraFocus(player.Index);

            // Generate a new camera modifier
            player.GenerateCameraModifier(false);

            // Zoom camera out
            player.ZoomOut();

            // Set selected
            player.Selected = true;

            // Focus player indicator
            player.FocusUIInterfaceObject(GamePlayer.UIInterfaceObject.PlayerIndicator);

            if ( player.PawnsAtEnd > 0 )
                player.FocusUIInterfaceObject(GamePlayer.UIInterfaceObject.PlayerIndicator2);

            if (player.PawnsAtEnd > 1)
                player.FocusUIInterfaceObject(GamePlayer.UIInterfaceObject.PlayerIndicator3);

            // Check if the game players current square is the start square or not.
            // If it is, then show tutorial 1 if it hasn't already.
            if (player.ActivePawn != null)
            {
                TutorialManager tutorials = TutorialManager.Instance;

                if (!tutorials.HasTutorialBeenShown(TutorialStage.StartTutorial))
                    player.ZoomInHalfWay();   

             /*   if (player.ActivePawn.CurrentSquare != null)
                {
                    Type squareType = player.ActivePawn.CurrentSquare.GetType();

                    if (squareType == typeof(StartSquare)) 
                    {
                      
                    }
                }*/
            }
        }

        public override void Update(GameTime deltaTime, GamePlayer player)
        {
            bool changeState = true;
            TutorialManager tutorials = TutorialManager.Instance;

            if (player.ActivePawn != null)
            {
                BoardSquare current = player.ActivePawn.CurrentSquare;

                if (current != null)
                {
                    if (!tutorials.HasTutorialBeenShown(TutorialStage.StartTutorial))
                    {
                        // If the camera is stationary
                        if (CameraManager.Instance.ActiveCamera != null)
                        {
                            if (!CameraManager.Instance.ActiveCamera.Tween)
                            {
                                // Show start tutorial
                                tutorials.ShowTutorial(TutorialStage.StartTutorial, Vector3.Zero, UIManager.TextureAlignment.eTopLeft);
                            }
                        }

                        // Set not to change state this cycle 
                        changeState = false;
                    }
                    else if (current.GetType().IsSubclassOf(typeof(ArithmeticSquare)))
                    {
                        if ( !tutorials.HasTutorialBeenShown(TutorialStage.ArithmeticSquares) )
                        {
                            // If the camera is stationary
                            if (CameraManager.Instance.ActiveCamera != null)
                            {
                                if (!CameraManager.Instance.ActiveCamera.Tween)
                                {
                                    // Show start tutorial
                                    tutorials.ShowTutorial(TutorialStage.ArithmeticSquares, Vector3.Zero, UIManager.TextureAlignment.eTopLeft);
                                }
                            }

                            // Set not to change state this cycle 
                            changeState = false;
                        }
                    }
                    else if (current.GetType() == typeof(IfSquare))
                    {
                        if (!tutorials.HasTutorialBeenShown(TutorialStage.IfSquare))
                        {
                            // If the camera is stationary
                            if (CameraManager.Instance.ActiveCamera != null)
                            {
                                if (!CameraManager.Instance.ActiveCamera.Tween)
                                {
                                    // Show start tutorial
                                    tutorials.ShowTutorial(TutorialStage.IfSquare, Vector3.Zero, UIManager.TextureAlignment.eTopLeft);
                                }
                            }

                            // Set not to change state this cycle 
                            changeState = false;
                        }
                    }
                    else if (current.GetType() == typeof(WhileSquare))
                    {
                        if (!tutorials.HasTutorialBeenShown(TutorialStage.WhileSquare))
                        {
                            // If the camera is stationary
                            if (CameraManager.Instance.ActiveCamera != null)
                            {
                                if (!CameraManager.Instance.ActiveCamera.Tween)
                                {
                                    // Show start tutorial
                                    tutorials.ShowTutorial(TutorialStage.WhileSquare, Vector3.Zero, UIManager.TextureAlignment.eTopLeft);
                                }
                            }

                            // Set not to change state this cycle 
                            changeState = false;
                        }
                    }
                    else if (current.GetType() == typeof(SwitchSquare))
                    {
                        if (!tutorials.HasTutorialBeenShown(TutorialStage.SwitchSquare))
                        {
                            // If the camera is stationary
                            if (CameraManager.Instance.ActiveCamera != null)
                            {
                                if (!CameraManager.Instance.ActiveCamera.Tween)
                                {
                                    // Show start tutorial
                                    tutorials.ShowTutorial(TutorialStage.SwitchSquare, Vector3.Zero, UIManager.TextureAlignment.eTopLeft);
                                }
                            }

                            // Set not to change state this cycle 
                            changeState = false;
                        }
                    }
                }
            }

            if (changeState)
            {
                // Change back to idle state
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
