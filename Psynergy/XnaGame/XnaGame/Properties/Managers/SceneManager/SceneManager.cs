using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace XnaGame
{
    class SceneManager : Singleton<SceneManager>
    {
        private SortedList<String, SceneNode> m_Scenes = new SortedList<String, SceneNode>();
        private SceneNode m_CurrentScene = null;

        public SceneManager()
        {

        }

        public override void Initialise()
        {
            // Add Scenes
            AddScene("GameScene");

            // Change Scenes
            ChangeScene("GameScene");

            // Add sprite to test scene
            m_CurrentScene.AddChild( new SpriteNode("testPic", "testPic", new Vector2()) );

            // Initialise
            m_CurrentScene.Initialise();

            base.Initialise();
        }

        public override void Load()
        {
            for (int i = 0; i < m_Scenes.Count; i++)
            {
                m_Scenes.ElementAt(i).Value.Load();
            }
        }

        public void AddScene(String name)
        {
            m_Scenes.Add( name, new SceneNode(name) );
        }

        public void ChangeScene(String name)
        {
            int index = -1;

            index =   m_Scenes.IndexOfKey( name );

            if (index >= 0)
                m_CurrentScene = m_Scenes.ElementAt(index).Value;
        }

        public override void Update(GameTime deltaTime)
        {
            if (m_CurrentScene != null)
                m_CurrentScene.Update(deltaTime);
        }

        public override void UnLoad()
        {
            m_CurrentScene.UnLoad();
        }

        public SceneNode CurrentScene { get { return m_CurrentScene; } set { m_CurrentScene = value; } }
    }
}
