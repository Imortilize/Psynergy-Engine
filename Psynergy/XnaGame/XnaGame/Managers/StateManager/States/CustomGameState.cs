using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;

using Psynergy;
using Psynergy.Graphics;
using Psynergy.Sound;
using Psynergy.Physics;
using Psynergy.Input;
using Psynergy.Camera;
using Psynergy.AI;
using Psynergy.Menus;

namespace XnaGame
{
    public class CustomGameState : GameState, IRegister<CustomGameState>
    {
        public CustomGameState() : base()
        {
        }

        public override void Initialise()
        {
            base.Initialise();

            // Initialise player manager
            new PlayerManager();
            PlayerManager.Instance.Initialise();

            // Create tutorial manager
            new TutorialManager();
            TutorialManager.Instance.Initialise();

            // Create question manager
            new QuestionManager();
            QuestionManager.Instance.Initialise();

            // Start camera
            m_StartCamera = "FixedThirdPerson";
        }

        public override void Load()
        {
            base.Load();

            // Load player manager
            PlayerManager.Instance.Load();

            // Load tutorial manager
            TutorialManager.Instance.Load();

            // Load question manager
            QuestionManager.Instance.Load();
        }

        public override void UnLoad()
        {
            base.UnLoad();

            // Unload the player manager
            PlayerManager.Instance.UnLoad();

            // Unload tutorial manager
            TutorialManager.Instance.UnLoad();

            // Unload question manager
            QuestionManager.Instance.UnLoad();
        }

        public override void Reset()
        {
            // Reset player manager
            PlayerManager.Instance.RemovePlayersFromScene();

            base.Reset();

            // Reset the players in this scene
            PlayerManager.Instance.ResetPlayersInScene(m_SceneManager.CurrentScene);

            // Reset the player manager
            PlayerManager.Instance.Reset();

            // Reset tutorial manager
            TutorialManager.Instance.Reset();

            // Reset question manager
            QuestionManager.Instance.Reset();
        }

        public override void OnEnter(GameObject objectRef)
        {
            // Reset game state
            Reset();

            // Set it so that the player can pause the game
            InputManager.Instance.ActivatePause();

            // Activate player one for now
            PlayerManager.Instance.SetActivePlayer(PlayerIndex.One);

            // Change to the fixed third person camera
            CameraManager.Instance.ChangeCamera(m_StartCamera);

            // Set camera focus onto player one for now
            BaseCamera camera = CameraManager.Instance.ActiveCamera;

            if (camera != null)
            {
                Camera3D camera3D = (camera as Camera3D);

                if (camera3D != null)
                {
                    // Reset camera start position
                    camera3D.Reset();

                    if (camera != null)
                    {
                        PlayerManager.Instance.SetInstantCameraFocus(PlayerIndex.One);

                        // Show player 1 turn
                        PlayerManager.Instance.ShowActivePlayer();
                    }
                }

                // Change game gravity
                PhysicsManager.Instance.SetGravity(Vector3.Down * 110);
            }

            MenuManager.Instance.CloseMenu();

            // Play game music
            SoundManager.Instance.PlayMusic("gamemusic");
        }

        public override void Update(GameTime deltaTime, GameObject objectRef)
        {
            if (!TutorialManager.Instance.TutorialPause && !QuestionManager.Instance.Pause)
            {
                if (m_SceneManager != null)
                {
                    // Update the current scene
                    m_SceneManager.Update(deltaTime);

                    // Update the player
                    PlayerManager.Instance.Update(deltaTime);
                }
            }

            // Update tutorials
            TutorialManager.Instance.Update(deltaTime);

            // Update questions
            QuestionManager.Instance.Update(deltaTime);
        }

        public override void Render(GameTime deltaTime, GameObject objectRef)
        {
            if (m_SceneManager != null)
            {
                // Update the current scene
                m_SceneManager.Render(deltaTime);

                // Update the player
                PlayerManager.Instance.Render(deltaTime);
            }
        }

        public override void OnExit(GameObject objectRef)
        {
            base.OnExit(objectRef);

            // Set all players inactive
            PlayerManager.Instance.SetAllPlayersInActive();
        }
    }
}
