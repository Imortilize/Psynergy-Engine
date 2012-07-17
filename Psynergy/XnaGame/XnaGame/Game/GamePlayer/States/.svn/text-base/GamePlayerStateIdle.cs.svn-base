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

using Psynergy;
using Psynergy.Input;
using Psynergy.Camera;
using Psynergy.Menus;

namespace XnaGame
{
    class GamePlayerStateIdle : GamePlayerState
    {
        private float m_WaitRollTimeReset = 1.0f;
        private float m_WaitRollTime = 0.0f;

        public GamePlayerStateIdle() : base()
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
            m_WaitRollTime = m_WaitRollTimeReset;
        }

        public override void OnEnter(GamePlayer player)
        {
            base.OnEnter(player);

            if (player.Selected)
            {
                // Zoom camera out
                player.ZoomOut();
            }

            m_WaitRollTime = 0.0f;

            if (player.Control == Player.PlayerControl.AI)
            {
                // Selected play code only
                if (player.Selected)
                {
                    if ((player.Pawns.Count - player.PawnsAtEnd) > 1)
                    {
                        int randomNum = RandomHelper.m_RandomGenerator.Next(0, 101);

                        if (randomNum <= 20)
                            player.FocusNextPawn();
                    }
                }

                m_WaitRollTime = m_WaitRollTimeReset;
            }
        }

        public override void Update(GameTime deltaTime, GamePlayer player)
        {
            foreach (GamePawn pawn in player.Pawns)
            {
                if (pawn.AnimationPlayer != null)
                {
                    // Show idle animation if done
                    if (pawn.AnimationPlayer.Done)
                        pawn.ShowIdle();
                    else if ( pawn.AnimationPlayer.CurrentClip != null )
                    {
                        if ( pawn.AnimationPlayer.CurrentClip.Name != "idle" )
                            pawn.ShowIdle();
                    }
                }
            }

            if (!TutorialManager.Instance.IsTutorialOpen())
            {
                // If the camera is stationary
                if (CameraManager.Instance.ActiveCamera != null)
                {
                    if (!CameraManager.Instance.ActiveCamera.Tween)
                    {
                        // Selected play code only
                        if (player.Selected)
                        {
                            // Active pawn only
                            if (player.ActivePawn != null)
                            {
                                bool rollDice = false;

                                if (player.Control == Player.PlayerControl.Human)
                                {
                                    if (InputManager.Instance.KeyPressed(Keys.Enter))
                                        rollDice = true;
                                }
                                else if (player.Control == Player.PlayerControl.AI)
                                {
                                    if ((m_WaitRollTime -= (float)deltaTime.ElapsedGameTime.TotalSeconds) <= 0.0f)
                                        rollDice = true;
                                }

                                // If roll dice is true play turn
                                if (rollDice)
                                {
                                    // Begin rolling the dice
                                    player.BeginRollDice();
                                }
                                else
                                    player.ActivePawn.ShowSelected();
                            }
                        }
                    }
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
