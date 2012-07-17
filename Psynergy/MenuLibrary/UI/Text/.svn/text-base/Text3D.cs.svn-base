using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Psynergy;
using Psynergy.Camera;
using Psynergy.Graphics;

namespace Psynergy.Menus
{
    public class Text3D : Text
    {
        private float m_VisibleTime = 2.0f;
        private float m_CurrentVisibleTime = 0.0f;

        private RenderTarget2D m_RenderTarget = null;

        public Text3D(String name, String text, Vector3 position3D, Color color, float scale)
            : base(name, text, color, scale)
        {
            Viewport viewPort = RenderManager.Instance.GraphicsDevice.Viewport;
            m_RenderTarget = new RenderTarget2D(RenderManager.Instance.GraphicsDevice, viewPort.Width, viewPort.Height);

            Position = position3D;
            Position2D = RenderManager.Instance.Project3DTo2D(Position, Matrix.Identity);
            m_Effect = new UIObjectEffectRiseUp(5.0f);
            m_Effect.SetUIObject(this);
            m_SpriteBatch = RenderManager.Instance.SpriteBatch;
            m_Depth = 0.25f;
        }

        public override void Update(GameTime deltaTime)
        {
            base.Update(deltaTime);

            // Measure string and center it ( test for now )
            if (m_Font != null)
            {
                Vector2 fontSize = m_Font.MeasureString(m_Text);
                
                Position2D = RenderManager.Instance.Project3DTo2D(Position, Matrix.Identity);
                //Position2D.X -= (fontSize.X * 0.5f);
                //Position2D.Y -= (fontSize.Y * 0.5f);
            }
        }

        protected override void OnIn(GameTime deltaTime)
        {
            m_CurrentVisibleTime = m_VisibleTime;
        }

        protected override void OnIdle(GameTime deltaTime)
        {
            if ((m_VisibleTime -= (float)deltaTime.ElapsedGameTime.TotalSeconds) <= 0.0f)
                SetActiveState(ActiveState.eOut);
        }

        protected override void OnOut(GameTime deltaTime)
        {
            base.OnOut(deltaTime);
        }

        public override void Render(GameTime deltaTime)
        {
            base.Render(deltaTime);
        }
    }
}
