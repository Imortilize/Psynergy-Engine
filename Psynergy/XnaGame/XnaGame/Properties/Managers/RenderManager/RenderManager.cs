using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace XnaGame
{
    class RenderManager : Singleton<RenderManager>
    {
        ContentManager m_ContentManager = null;
        GraphicsDeviceManager m_Graphics = null;
        SpriteBatch m_SpriteBatch = null;

        public RenderManager()
        {
        }

        public override void Initialise()
        {
         
        }

        public Texture2D LoadTexture2D( String assetName )
        {
            return m_ContentManager.Load<Texture2D>(assetName);
        }

        public override void Update(GameTime deltaTime)
        {
            if ( SceneManager.Instance.CurrentScene != null )
                SceneManager.Instance.CurrentScene.Update( deltaTime );
        }

        public void Render(GameTime deltaTime)
        {
            if (SceneManager.Instance.CurrentScene != null)
                SceneManager.Instance.CurrentScene.Render(deltaTime);
        }

        public override void UnLoad()
        {
        }

        public ContentManager ContentManager { get { return m_ContentManager; } set { m_ContentManager = value; } }
        public GraphicsDeviceManager GraphicsDeviceManager { get { return m_Graphics; } set { m_Graphics = value; } }
        public SpriteBatch SpriteBatch { get { return m_SpriteBatch; } set { m_SpriteBatch = value; } }
    }
}
