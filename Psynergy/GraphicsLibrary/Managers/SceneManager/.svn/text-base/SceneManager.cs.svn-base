using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

/* Physics library */
using Psynergy.Physics;

namespace Psynergy.Graphics
{
    public class SceneManager : Singleton<SceneManager>
    {
        private SpriteBatch m_SpriteBatch = null;

        private SortedList<String, SceneResource> m_Scenes = new SortedList<String, SceneResource>();   // Holds all the scenes for all the scene fragments
        private Scene m_CurrentScene = null;    // Holds the current scene

        public SceneManager()
        {

        }

        public override void Initialise()
        {
            // TO BE CLEANED UP IN A GAME FRAGMENT LATER ON
            // Was loading a single scene, now moving forward to load multiple scenes for a single scene fragment 
            // ( hacky but further forward then before )
            String filename = "Resources/GameScenes.xml";

            // Load scene fragment
            AddSceneFragment("GameScenes", filename);

            // Get the spritebatch
            if(RenderManager.Instance != null )
                m_SpriteBatch = RenderManager.Instance.SpriteBatch;

            Debug.Assert( m_SpriteBatch != null, "Sprite batch not loaded on the scene manager" );

            base.Initialise();
        }

        public override void Load()
        {
            // Load all scene fragments accordingly
            foreach (SceneResource sceneFragment in m_Scenes.Values)
                sceneFragment.Load();
        }

        public override void Reset()
        {
            base.Reset();

            if (m_CurrentScene != null)
                m_CurrentScene.Reset();
        }

        public void AddSceneFragment(String name, String fileName)
        {
            // Add the scene to the sorted list
            m_Scenes.Add(name, new SceneResource(fileName));
        }

        public void ChangeScene(String name)
        {
            m_CurrentScene = FindScene(name);
        }

        public Scene FindScene(String name)
        {
            Scene scene = null;

            if (m_Scenes != null)
            {
                // Go through all the scene fragments and search for this game scene
                foreach (SceneResource sceneFragment in m_Scenes.Values)
                {
                    // Search for the scene in the scene fragments scenes
                    Scene next = sceneFragment.GetScene(name);

                    // If the scene was found
                    if (next != null)
                        scene = next;
                }
            }

            return scene;
        }

        public override void Update(GameTime deltaTime)
        {
            // Update the scene
            if (m_CurrentScene != null)
            {
                m_CurrentScene.Update(deltaTime);

                // Update any physics engine being used for this scene
                PhysicsManager.Instance.Update(deltaTime);
            }
        }

        public override void Render(GameTime deltaTime)
        {
            Debug.Assert(m_CurrentScene != null, "Current scene hasn't been set, it can not be null!");

            if (m_CurrentScene != null)
                 m_CurrentScene.Render(deltaTime);
        }

        public override void UnLoad()
        {
            if ( m_CurrentScene != null )
                m_CurrentScene.UnLoad();
        }

        public Scene CurrentScene { get { return m_CurrentScene; } set { m_CurrentScene = value; } }
    }
}
