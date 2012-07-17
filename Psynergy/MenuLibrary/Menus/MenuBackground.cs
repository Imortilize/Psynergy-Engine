using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

/* Main Library */
using Psynergy;

/* Sound Library */
using Psynergy.Sound;

/* Graphics Library */
using Psynergy.Graphics;

namespace Psynergy.Menus
{
    public class MenuBackground : SpriteNode
    {
        #region Factory Property setting
        protected override void ClassProperties(Factory factory)
        {
            factory.RegisterString("music", "Music");

            base.ClassProperties(factory);
        }

        #endregion

        // Music reference
        protected String m_MusicName = "";

        public MenuBackground() : base()
        {
            m_RootFolderName = "Textures/Menus/";
        }

        public MenuBackground(String name, String assetName, Vector3 position) : base(name, assetName, position)
        {
            m_RootFolderName = "Textures/Menus/";
        }

        public override void Initialise()
        {
            base.Initialise();

            // Background should be rendered behind everything else
            m_RenderDepth = 1.0f;
        }

        public override void Load()
        {
            base.Load();
        }

        public override void Update(GameTime deltaTime)
        {
            base.Update(deltaTime);
        }

        public override void Render(GameTime deltaTime)
        {
            Vector2 view = RenderManager.Instance.BaseResolution;
            Rectangle fullscreen = new Rectangle(0, 0, (int)view.X, (int)view.Y);

            m_SpriteBatch.Draw(m_CurrentTexture, GetPos2D(), fullscreen, m_ActualColor, 0.0f, Vector2.Zero, GetScale2D(), SpriteEffects.None, MathHelper.Clamp(m_RenderDepth, 0.0f, 1.0f));
        }

        public void PlayMusic()
        {
            SoundManager.Instance.PlayMusic(m_MusicName);
        }

        #region Property Set / Gets
        public String Music { get { return m_MusicName; } set { m_MusicName = value; } }
        #endregion
    }
}
