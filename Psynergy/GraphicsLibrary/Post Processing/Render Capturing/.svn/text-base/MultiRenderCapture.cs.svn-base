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
    public class MultiRenderCapture : RenderCapture
    {
        public MultiRenderCapture(GraphicsDevice graphicsDevice)
            : base(graphicsDevice)
        {
        }

        public override void Begin()
        {
            if (m_RenderTargets.Count > 1)
            {
                if (m_GraphicsDevice != null)
                    m_GraphicsDevice.SetRenderTargets(m_RenderTargets[0], m_RenderTargets[1]);
            }
        }

        public override void End()
        {
            if (m_GraphicsDevice != null)
                m_GraphicsDevice.SetRenderTargets(null);
        }

        public void SetEffectParameter(int index, Effect effect, String parameter)
        {
            Debug.Assert(index < m_RenderTargets.Count, "[ERROR] - TRYING TO GET A RENDER CAPTURE TEXTURE FROM INDEX " + index + " AND IT DOESN'T EXIST!");

            if (index < m_RenderTargets.Count)
            {
                if ((effect != null) && (parameter != ""))
                {
                    if (effect.Parameters[parameter] != null)
                        effect.Parameters[parameter].SetValue(m_RenderTargets[index]);
                }
            }
        }

        public Texture2D GetTexture(int index)
        {
            Texture2D toRet = null;

            Debug.Assert(index < m_RenderTargets.Count, "[ERROR] - TRYING TO GET A RENDER CAPTURE TEXTURE FROM INDEX " + index + " AND IT DOESN'T EXIST!");

            if (index < m_RenderTargets.Count)
                toRet = m_RenderTargets[index];

            return toRet;
        }
    }
}
