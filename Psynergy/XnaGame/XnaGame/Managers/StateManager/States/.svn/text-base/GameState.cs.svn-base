using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;

/* Main Library */
using Psynergy;

/* Graphics Library */
using Psynergy.Graphics;

using Psynergy.Sound;
using Psynergy.Physics;
using Psynergy.Input;
using Psynergy.Camera;
using Psynergy.AI;
using Psynergy.Menus;

namespace XnaGame
{
    public class GameState : State<GameObject>
    {
        // Scene Manager
        SceneManager m_SceneManager = null;

        public GameState() : base()
        {
        }

        public override void Initialise()
        {
            // Initialise Scene Manager
            m_SceneManager = SceneManager.Instance;
            Debug.Assert(m_SceneManager != null, "Scene manager should not be null!");

            m_SceneManager.Initialise();

            // Initialise player manager
            new PlayerManager();
            PlayerManager.Instance.Initialise();

            // Create tutorial manager
            new TutorialManager();
            TutorialManager.Instance.Initialise();

            // Create question manager
            new QuestionManager();
            QuestionManager.Instance.Initialise();
        }

        public override void Load()
        {
            // Load the scenes
            m_SceneManager.Load();

            // Load player manager
            PlayerManager.Instance.Load();

            // Load tutorial manager
            TutorialManager.Instance.Load();

            // Load question manager
            QuestionManager.Instance.Load();
        }

        public override void UnLoad()
        {
            // Unload the scenes
            m_SceneManager.UnLoad();

            // Unload the player manager
            PlayerManager.Instance.UnLoad();

            // Unload tutorial manager
            TutorialManager.Instance.UnLoad();

            // Unload question manager
            QuestionManager.Instance.UnLoad();
        }

        public override void Reset()
        {
            // Reset render manager ( particles at the moment )
            RenderManager.Instance.ResetParticles();

            // Reset player manager
            PlayerManager.Instance.RemovePlayersFromScene();

            // Change to the default start scene for the game
            m_SceneManager.ChangeScene("GameScene");

            // Reset the scene accordingly
            m_SceneManager.Reset();

            // Reset the players in this scene
            PlayerManager.Instance.ResetPlayersInScene(m_SceneManager.CurrentScene);

            // Reset user interface stuff
            UIManager.Instance.Reset();

            // Reset the player manager
            PlayerManager.Instance.Reset();

            // Reset tutorial manager
            TutorialManager.Instance.Reset();

            // Reset question manager
            QuestionManager.Instance.Reset();
        }

        public override void OnEnter(GameObject objectRef)
        {
            base.OnEnter(objectRef);

            // Reset game state
            Reset();

            // Set it so that the player can pause the game
            InputManager.Instance.ActivatePause();

            // Activate player one for now
            PlayerManager.Instance.SetActivePlayer(PlayerIndex.One);

            // Change to the fixed third person camera
            CameraManager.Instance.ChangeCamera("FixedThirdPerson");

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

            // Show player 1 turn
            //PlayerManager.Instance.ShowActivePlayer();

            MenuManager.Instance.CloseMenu();

            // Play game music
            SoundManager.Instance.PlayMusic("gamemusic");
        }

        public override void Update(GameTime deltaTime, GameObject objectRef)
        {
            base.Update(deltaTime, objectRef);

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
            base.Render(deltaTime, objectRef);

            if (m_SceneManager != null)
            {
                // Update the current scene
                m_SceneManager.Render(deltaTime);

                // Update the player
                PlayerManager.Instance.Render(deltaTime);
            }
        }

        protected override bool CloseChecks()
        {
            return true;
        }

        public override void OnExit(GameObject objectRef)
        {
            base.OnExit(objectRef);

            // Set it so that the player can no longer pause the game
            InputManager.Instance.DeactivatePause();

            // Set all players inactive
            PlayerManager.Instance.SetAllPlayersInActive();

            // Set default camera
            CameraManager.Instance.SetDefaultCamera();
        }
    }
}
