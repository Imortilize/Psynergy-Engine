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
    public class RenderCapture
    {
        protected List<RenderTarget2D> m_RenderTargets = new List<RenderTarget2D>();
        protected GraphicsDevice m_GraphicsDevice;

        public RenderCapture(GraphicsDevice graphicsDevice)
        {
            m_GraphicsDevice = graphicsDevice;
        }

        public void AddRenderTarget()
        {
            if (m_GraphicsDevice != null)
                AddRenderTarget(m_GraphicsDevice.PresentationParameters.BackBufferWidth, m_GraphicsDevice.PresentationParameters.BackBufferHeight, SurfaceFormat.Color, DepthFormat.Depth24);
        }

        public void AddRenderTarget(SurfaceFormat surfaceFormat, DepthFormat depthFormat)
        {
            if (m_GraphicsDevice != null)
                AddRenderTarget(m_GraphicsDevice.PresentationParameters.BackBufferWidth, m_GraphicsDevice.PresentationParameters.BackBufferHeight, surfaceFormat, depthFormat);
        }

        public void AddRenderTarget(int width, int height, SurfaceFormat surfaceFormat, DepthFormat depthFormat)
        {
            if (m_GraphicsDevice != null)
                m_RenderTargets.Add(new RenderTarget2D(m_GraphicsDevice, width, height, false, surfaceFormat, depthFormat));
        }

        public virtual void Reset()
        {
            if ( m_GraphicsDevice != null )
            {
                for (int i = 0; i < m_RenderTargets.Count; i++)
                {
                    RenderTarget2D renderTarget = m_RenderTargets[i];
                    PresentationParameters pp = m_GraphicsDevice.PresentationParameters;

                    // Resize accordingly
                    m_RenderTargets[i] = new RenderTarget2D(m_GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, false, renderTarget.Format, renderTarget.DepthStencilFormat);
                }
            }
        }

        public virtual void Begin()
        {
            if (m_RenderTargets.Count > 0)
            {
                if (m_GraphicsDevice != null)
                    m_GraphicsDevice.SetRenderTarget(m_RenderTargets[0]);
            }
        }

        // Stop capturing
        public virtual void End()
        {
            if (m_GraphicsDevice != null)
                m_GraphicsDevice.SetRenderTarget(null);
        }

        public void SetEffectParameter(Effect effect, String parameter)
        {
            if (m_RenderTargets.Count > 0)
            {
                if ((effect != null) && (parameter != ""))
                {
                    if (effect.Parameters[parameter] != null)
                        effect.Parameters[parameter].SetValue(m_RenderTargets[0]);
                }
            }
        }

        // Returns what was captured
        public Texture2D GetTexture()
        {
            Texture2D target = null;

            if (m_RenderTargets.Count > 0)
                target = m_RenderTargets[0];

            return target;
        }

        public Vector2 GetTargetSize()
        {
            return new Vector2(m_RenderTargets[0].Width, m_RenderTargets[0].Height);
        }

        #region Property Set / Gets
        public int NumRenderTargets { get { return m_RenderTargets.Count; } }
        #endregion
    }
}
