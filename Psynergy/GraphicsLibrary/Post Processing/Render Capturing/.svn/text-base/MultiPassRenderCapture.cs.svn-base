using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Psynergy.Graphics
{
    public class MultiPassRenderCapture : MultiRenderCapture
    {
        private int m_PassIndex = 0;

        public MultiPassRenderCapture(GraphicsDevice graphicsDevice) : base(graphicsDevice)
        {
        }

        public override void Begin()
        {
            if (m_PassIndex >= m_RenderTargets.Count)
            {
                // Reset back to the first pass
                m_PassIndex = 0;
            }

            // Set render target
            if (m_GraphicsDevice != null)
                m_GraphicsDevice.SetRenderTarget(m_RenderTargets[m_PassIndex]);

            // Increment what pass the render capture is on
            m_PassIndex++;
        }

        public override void End()
        {
            if (m_GraphicsDevice != null)
                m_GraphicsDevice.SetRenderTarget(null);
        }

        #region Get / Set
        public int PassIndex { get { return m_PassIndex; } }
        #endregion
    }
}
