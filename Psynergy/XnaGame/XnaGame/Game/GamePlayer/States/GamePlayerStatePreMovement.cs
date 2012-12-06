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
    class GamePlayerStatePreMovement : GamePlayerState
    {
        public GamePlayerStatePreMovement() : base()
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

            // Zoom camera in and to lower pitch
            player.ZoomIn();
        }

        public override void Update(GameTime deltaTime, GamePlayer player)
        {
            if (player.ActivePawn != null)
            {
                if (player.CurrentDiceRoll == 0)
                {
                    BaseCamera camera = CameraManager.Instance.ActiveCamera;

                    if (!camera.Tween)
                    {
                        if (camera.GetType().IsSubclassOf(typeof(Camera3D)))
                        {
                            if (camera.GetType() == typeof(FixedThirdPersonCamera))
                            {
                                FixedThirdPersonCamera fixedCamera = (camera as FixedThirdPersonCamera);

                                float zoomDistance = fixedCamera.GetDistanceFromDesiredZoom();

                                if ((zoomDistance <= 50.0f) && (zoomDistance >= -50.0f))
                                {
                                    /* Begin the pawn moving */
                                    // Save the dice roll determined
                                    player.OriginalDiceRoll = player.CurrentDiceRoll = player.Data.diceRoll;

                                    // Display text in 3D
                                    Color textColor = Color.Yellow;
                                    String rollText = "";

                                    if (player.CurrentDiceRoll > 0)
                                    {
                                        rollText = "+";
                                        textColor = Color.ForestGreen;
                                    }
                                    else if (player.CurrentDiceRoll < 0)
                                    {
                                        rollText = "-";
                                        textColor = Color.OrangeRed;
                                    }

                                    rollText += player.CurrentDiceRoll.ToString();

                                    if (player.CurrentDiceRoll != 1)
                                        rollText += " Squares";
                                    else
                                        rollText += " Square";

                                    String colorText = "red";

                                    if (player.Color == GamePlayer.PlayerColor.Blue)
                                        colorText = "blue";

                                    // Add text to it
                                    colorText += "text";

                                    // Add text
                                    UIManager.Instance.AddText3DObject(colorText, rollText, (player.ActivePawn.transform.Position + new Vector3(0, +10, 0)), textColor, 1.5f);
                                    //UIManager.Instance.AddText3DObject(colorText + "outline", rollText, (player.ActivePawn.Position + new Vector3(-0.05f, +10, -0.05f)), Color.White, 2.05f); 

                                    // Generate a new camera modifier
                                    player.GenerateCameraModifier(true);

                                    if (player.CurrentDiceRoll != 0)
                                    {
                                        // Move to the next square assuming the dice roll returned grater than 0 ( should do )
                                        if (player.CurrentDiceRoll > 0)
                                        {
                                            // Check whether to go to the jump state first or not
                                            if (player.Data.route != BoardSquare.Route.eDefault)
                                                player.ChangeState_Jump();
                                            else
                                            {
                                                player.MoveToNextSquare();

                                                // Swap to movement state
                                                player.ChangeState_Movement();
                                            }
                                        }
                                        else
                                        {
                                            player.MoveToPreviousSquare();

                                            // Swap to movement state
                                            player.ChangeState_Movement();
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
                            }
                        }
                    }
                    else
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
