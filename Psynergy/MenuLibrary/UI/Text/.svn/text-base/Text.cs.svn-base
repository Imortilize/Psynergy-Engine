using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Psynergy;
using Psynergy.Graphics;

namespace Psynergy.Menus
{
    public class Text : UIObject
    {
        // Sprite fonts
        protected SpriteFont m_Font = null;
        protected String m_Text = "";
        protected Color m_Color = Color.Red;
        protected Vector2 Position2D = Vector2.Zero;
        protected float m_Depth = 0.0f;
        protected bool m_UseOutline = true;

        public Text(String name, String text)
            : base(name)
        {
            m_Font = RenderManager.Instance.LoadFont("defaultFont");
            m_Text = text;
            m_Depth = 0.35f;
            m_SpriteBatch = RenderManager.Instance.SpriteBatch;
        }

        public Text(String name, String text, Color color, float scale)
            : base(name)
        {
            m_Font = RenderManager.Instance.LoadFont("defaultFont");
            m_Text = text;
            m_Color = color;
            m_Depth = 0.35f;
            m_SpriteBatch = RenderManager.Instance.SpriteBatch;

            Scale = new Vector3(scale, scale, scale);
        }

        public Text(String name, String text, Color color, Vector2 position, float scale)
            : base(name)
        {
            m_Font = RenderManager.Instance.LoadFont("defaultFont");
            m_Text = text;
            Position2D = position;
            m_Color = color;
            m_Depth = 0.35f;
            m_SpriteBatch = RenderManager.Instance.SpriteBatch;

            Scale = new Vector3(scale, scale, scale);
        }

        public override void Render(GameTime deltaTime)
        {
            if ((m_SpriteBatch != null) && (m_Font != null))
            {
                float scale = ((Scale.X + Scale.Y + Scale.Z) / 3);

                Vector2 origin = new Vector2(m_Font.MeasureString(m_Text).X / 2, m_Font.MeasureString(m_Text).Y / 2);

                if (m_UseOutline)
                {
                    Color borderColor = Color.White;

                    if ((m_Color == Color.White) || (m_Color == Color.Yellow))
                        borderColor = Color.Black;

                    //These 4 draws are the background of the text and each of them have a certain displacement each way.
                    m_SpriteBatch.DrawString(m_Font, m_Text, Position2D + new Vector2(1 * scale, 1 * scale), borderColor, 0, origin, scale, SpriteEffects.None, 0.4f);
                    m_SpriteBatch.DrawString(m_Font, m_Text, Position2D + new Vector2(-1 * scale, -1 * scale), borderColor, 0, origin, scale, SpriteEffects.None, 0.4f);
                    m_SpriteBatch.DrawString(m_Font, m_Text, Position2D + new Vector2(-1 * scale, 1 * scale), borderColor, 0, origin, scale, SpriteEffects.None, 0.4f);
                    m_SpriteBatch.DrawString(m_Font, m_Text, Position2D + new Vector2(1 * scale, -1 * scale), borderColor, 0, origin, scale, SpriteEffects.None, 0.4f);
                }

                m_SpriteBatch.DrawString(m_Font, m_Text, Position2D, m_Color, 0, origin, scale, SpriteEffects.None, m_Depth);
            }
        }

        public void SetText(String text)
        {
            m_Text = text;
        }
    }
}
