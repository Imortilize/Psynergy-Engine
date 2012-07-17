using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace XnaGame
{
    class SpriteNode : VisualNode
    {
        private Texture2D m_Texture = null;
        private String m_TextureName = "";

        private Vector2 m_Position = new Vector2( 0.0f, 0.0f);

        GraphicsDeviceManager m_Graphics = null;
        SpriteBatch m_SpriteBatch = null;

        public SpriteNode(String name, String assetName, Vector2 position) : base(name)
        {
            m_TextureName = assetName;
            m_Position = position;
        }

        public override void Initialise()
        {
            // Get the graphics related objects
            m_Graphics = RenderManager.Instance.GraphicsDeviceManager;

            base.Initialise();
        }

        public override void Load()
        {
            // Get the sprite batch related object
            m_SpriteBatch = RenderManager.Instance.SpriteBatch;

            // Check a texture name exists
            if ( m_TextureName != null )
            {
                // Now try and get the texture from the content pool
                m_Texture = RenderManager.Instance.LoadTexture2D(m_TextureName);
            }
        }

        public override void Reset()
        {
        }

        public override void Update( GameTime deltaTime )
        {
        }

        public override void Render( GameTime deltaTime )
        {
            // Draw the sprite.
            m_SpriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            m_SpriteBatch.Draw(m_Texture, m_Position, Color.White);
            m_SpriteBatch.End();

            base.Render( deltaTime );
        }
    }
}
