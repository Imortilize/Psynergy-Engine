using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Psynergy.Graphics
{
    public class EdgeDetection : PostProcessor<BasicProperties>
    {
        private Texture2D m_DepthMap = null;

        public EdgeDetection(GraphicsDevice graphicsDevice, Effect effect)
            : base(graphicsDevice, effect, new BasicProperties())
        {
        }

        protected override void CreateRenderCapture()
        {
            m_RenderCapture = new RenderCapture(m_GraphicsDevice);
            m_RenderCapture.AddRenderTarget(SurfaceFormat.Color, DepthFormat.None);
        }

        public override void Draw(Texture2D texture)
        {
            // Depth of field reies on the face we have a graphics device, an effect assigned 
            // a blur post processor and a merge bloom processor in able to effectively work
            if ((m_GraphicsDevice != null) && (Effect != null))
            {
                base.Draw(texture);
            }
        }

        #region Property Set / Gets
        public Texture2D DepthMap { set { m_DepthMap = value; } }
        #endregion

        #region Set / Get
        #endregion
    }
}
