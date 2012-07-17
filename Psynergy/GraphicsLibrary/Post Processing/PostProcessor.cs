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
    public class PostProcessor<T> : AbstractPostProcessor where T : AbstractPostProcessorProperties
    {
        // GraphicsDevice and SpriteBatch for drawing
        protected GraphicsDevice m_GraphicsDevice;
        protected SpriteBatch m_SpriteBatch;

        // Render capture 
        protected RenderCapture m_RenderCapture;

        // Abstracted properties
        protected T m_Properties;

        // Render effect
        private Effect m_Effect = null;

        public PostProcessor(GraphicsDevice graphicsDevice, T newTObject)
        {
            m_GraphicsDevice = graphicsDevice;

            // Create graphics settings
            CreateGraphicsDeviceSettings();

            // CreateProperties
            CreateProperties(newTObject);
        }

        public PostProcessor(GraphicsDevice graphicsDevice, Effect Effect, T newTObject) : base()
        {
            this.Effect = Effect;

            m_GraphicsDevice = graphicsDevice;

            // Create graphics settings
            CreateGraphicsDeviceSettings();

            // CreateProperties
            CreateProperties(newTObject);
        }

        private void CreateGraphicsDeviceSettings()
        {
            if (m_GraphicsDevice != null)
            {
                if (m_SpriteBatch == null)
                    m_SpriteBatch = new SpriteBatch(m_GraphicsDevice);

                // Create render capture
                CreateRenderCapture();
            }
        }

        protected virtual void CreateRenderCapture()
        {
            m_RenderCapture = new RenderCapture(m_GraphicsDevice);
            m_RenderCapture.AddRenderTarget();
        }

        protected void CreateProperties(T newTObject)
        {
            m_Properties = newTObject;
        }

        public virtual void Reset()
        {
            // Reset render capture for screen resizing
            m_RenderCapture.Reset();
        }

        // Draws the input texture using the pixel shader postprocessor
        public override void Draw(Texture2D input)
        {
            if ((m_GraphicsDevice != null) && (m_RenderCapture != null))
            {
                m_RenderCapture.Begin();

                // Clear device
                ClearBuffer();

                if (input != null)
                {
                    if (m_SpriteBatch != null)
                    {
                        // Initialize the spritebatch and effect
                        m_SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);

                        if (Effect != null)
                            Effect.CurrentTechnique.Passes[0].Apply();

                        // Set any sampler states required to make this processor work with the render targets set up
                        SetSamplerStates();

                        // Draw the input texture
                        m_SpriteBatch.Draw(input, Vector2.Zero, Color.White);

                        // End the spritebatch and effect
                        m_SpriteBatch.End();

                        // Clean up render states changed by the spritebatch
                        m_GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                        m_GraphicsDevice.BlendState = BlendState.Opaque;
                    }
                }
                else
                {
                    if (Effect != null)
                        Effect.CurrentTechnique.Passes[0].Apply();

                    // Set any sampler states required to make this processor work with the render targets set up
                    SetSamplerStates();

                    // Render full screen quad
                    QuadRenderer quadRenderer = new QuadRenderer();
                    quadRenderer.RenderQuad(m_GraphicsDevice, -Vector2.One, Vector2.One);
                }

                m_RenderCapture.End();
            }
        }

        protected virtual void ClearBuffer()
        {
            m_GraphicsDevice.Clear(Color.Black);
        }

        protected virtual void SetSamplerStates()
        {
        }

        public Texture2D GetProcessedImage(int index)
        {
            Texture2D toRet = null;

            if (index < m_RenderCapture.NumRenderTargets)
            {
                Type renderCaptureType = m_RenderCapture.GetType();

                if ((renderCaptureType == typeof(MultiRenderCapture)) || (renderCaptureType.IsSubclassOf(typeof(MultiRenderCapture))))
                {
                    MultiRenderCapture multiRenderCapture = (m_RenderCapture as MultiRenderCapture);

                    toRet = multiRenderCapture.GetTexture(index);
                }
                else
                    toRet = m_RenderCapture.GetTexture();
            }

            return toRet;
        }

        #region Setter Functions
        protected virtual void SetEffectParameters()
        {
        }
        #endregion


        #region Property Set / Gets
        public bool Enabled
        {
            get { return m_Properties.Enabled; }
            set { m_Properties.Enabled = value; }
        }

        // Pixel shader
        public Effect Effect 
        {
            get { return m_Effect; }
            set
            {
                m_Effect = value;
                
                // Set effect parameters from properties up front
                SetEffectParameters();
            }
        }

        // Texture to process
        public Texture2D ProcessedImage
        { 
            get 
            {
                Texture2D toRet = m_RenderCapture.GetTexture();
                int numRenderTargets = m_RenderCapture.NumRenderTargets;

                // If there are more render targets used then return the last one as 
                // this is the render target last rendered into
                Type renderCaptureType = m_RenderCapture.GetType();

                if ((renderCaptureType == typeof(MultiRenderCapture)) || (renderCaptureType.IsSubclassOf(typeof(MultiRenderCapture))))
                {
                    MultiRenderCapture multiRenderCapture = (m_RenderCapture as MultiRenderCapture);

                    toRet = multiRenderCapture.GetTexture(numRenderTargets - 1);
                }
                else
                    toRet = m_RenderCapture.GetTexture();

                return toRet; 
            }
        }

        public override Texture2D FinalImage
        {
            get
            {
                return ProcessedImage;
            }
        }
        #endregion
    }
}
