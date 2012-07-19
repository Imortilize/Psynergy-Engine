using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;

using Psynergy.Graphics;
using Psynergy.Sound;
using Psynergy.Physics;
using Psynergy.Input;
using Psynergy.Camera;
using Psynergy.AI;
using Psynergy.Menus;

namespace Psynergy
{
    public class GameState : State<GameObject>, IRegister<GameState>
    {
        // Scene Manager
        protected SceneManager m_SceneManager = null;
        protected String m_StartCamera = "Default";

        public GameState()
            : base()
        {
        }

        public override void Initialise()
        {
            // Initialise Scene Manager
            m_SceneManager = SceneManager.Instance;
            Debug.Assert(m_SceneManager != null, "Scene manager should not be null!");

            if ( m_SceneManager != null )
                m_SceneManager.Initialise();
        }

        public override void Load()
        {
            // Load the scenes
            if (m_SceneManager != null)
                m_SceneManager.Load();
        }

        public override void UnLoad()
        {
            // Unload the scenes
            if (m_SceneManager != null)
                m_SceneManager.UnLoad();
        }

        public override void Reset()
        {
            // Reset render manager ( particles at the moment )
            RenderManager.Instance.ResetParticles();

            // Change to the default start scene for the game
            if (m_SceneManager != null)
            {
                m_SceneManager.ChangeScene("GameScene");

                // Reset the scene accordingly
                m_SceneManager.Reset();
            }

            // Reset user interface stuff
            UIManager.Instance.Reset();
        }

        public override void OnEnter(GameObject objectRef)
        {
            base.OnEnter(objectRef);

            // Reset game state
            Reset();

            // Set it so that the player can pause the game
            InputManager.Instance.ActivatePause();

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
                }

                // Change game gravity
                PhysicsManager.Instance.SetGravity(Vector3.Down * 110);
            }

            MenuManager.Instance.CloseMenu();
        }

        public override void Update(GameTime deltaTime, GameObject objectRef)
        {
            base.Update(deltaTime, objectRef);

            if (m_SceneManager != null)
            {
                // Update the current scene
                m_SceneManager.Update(deltaTime);
            }
        }

        public override void Render(GameTime deltaTime, GameObject objectRef)
        {
            base.Render(deltaTime, objectRef);

            if (m_SceneManager != null)
            {
                // Update the current scene
                m_SceneManager.Render(deltaTime);
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

            // Set default camera
            CameraManager.Instance.SetDefaultCamera();
        }
    }
}
