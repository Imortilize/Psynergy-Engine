using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Psynergy.Graphics
{
    public class SpriteRenderer : PsynergyRenderer
    {
        private SpriteBatch m_SpriteBatch = null;
        private Matrix m_GlobalTransformation = Matrix.Identity;

        // List of sprites to draw
        private List<SpriteNode> m_Sprites = new List<SpriteNode>();

        public SpriteRenderer(ContentManager contentManager, GraphicsDeviceManager graphicsDevice) : base(contentManager, graphicsDevice)
        {
        }

        public override void Initialise()
        {
            base.Initialise();
        }

        protected override void SetupRenderTargets()
        {
            base.SetupRenderTargets();
        }

        public override void Reset()
        {
            base.Reset();
        }

        public override void Load()
        {
            base.Load();

            // Get the sprite batch
            m_SpriteBatch = RenderManager.Instance.SpriteBatch;

            // Calculate global transformation
            CalculateGlobalTransformation();
        }

        private void CalculateGlobalTransformation()
        {
            // Calculate the  sprite batch global transformation
            GraphicsDevice device = RenderManager.Instance.GraphicsDevice;
            Vector2 baseResolution = RenderManager.Instance.BaseResolution;
            float horScaling = (float)device.PresentationParameters.BackBufferWidth / baseResolution.X;
            float verScaling = (float)device.PresentationParameters.BackBufferHeight / baseResolution.Y;
            Vector3 screenScalingFactor = new Vector3(horScaling, verScaling, 1);

            // Scaler
            m_GlobalTransformation = Matrix.CreateScale(screenScalingFactor);
        }

        #region Render Loop
        public override void Begin()
        {
            base.Begin();
        }

        public override void Draw(GameTime deltaTime)
        {
            if (GraphicsDevice != null)
            {
                // Begin a seperate sprite batch for menus
                m_SpriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, null, null, null, m_GlobalTransformation);

                // Render sprite
                foreach (SpriteNode sprite in m_Sprites)
                    sprite.Render(deltaTime);

                // End the sprite batch
                m_SpriteBatch.End();
            }
        }

        public override void End()
        {
            base.End();
        }
        #endregion

        #region Defer functions
        protected override void DeferRenderable(RenderNode renderable)
        {
            // Only accepts models right now for this renderer
            Type type = renderable.GetType();

            if ((type == typeof(SpriteNode)) || (type.IsSubclassOf(typeof(SpriteNode))))
            {
                SpriteNode spriteNode = (renderable as SpriteNode);

                if (spriteNode != null)
                {
                    // Add to sprite pool
                    m_Sprites.Add(spriteNode);
                }
            }
        }
        #endregion

        #region PostProcessing
        protected override Texture2D PostProcess(Texture2D inTexture)
        {
            Texture2D texture = base.PostProcess(inTexture);

            if (texture != null)
            {
            }

            // Send post processed texture back
            return texture;
        }
        #endregion

        #region Rendering Technique Setters
        public override void UseShadows(bool use)
        {
            Console.WriteLine("[WARNING] - SHADOW MAPS NOT SUPPORTED ON THE SPRITE RENDERER!");
            EnableShadowMapping = false;
        }

        public override void SetToneMappingProperties(ToneMappingProperties properties)
        {
            Console.WriteLine("[WARNING] - TONE MAPPING IS NOT SUPPORTED ON THE SPRITE RENDERER!");
        }

        public override void SetFXAAProperties(FXAAProperties properties)
        {
            Console.WriteLine("[WARNING] - FXAA IS NOT SUPPORTED ON THE SPRITE RENDERER!");
        }

        public override void SetDepthOfFieldProperties(DepthOfFieldProperties properties)
        {
            Console.WriteLine("[WARNING] - DEPTH OF FIELD IS NOT SUPPORTED ON THE SPRITE RENDERER!");
        }

        public override void SetBloomProperties(BloomProperties properties)
        {
            Console.WriteLine("[WARNING] - BLOOM IS NOT SUPPORTED ON THE SPRITE RENDERER!");
        }

        public override void SetFilmGrainProperties(FilmGrainProperties properties)
        {
            Console.WriteLine("[WARNING] - FILM GRAIN IS NOT SUPPORTED ON THE SPRITE RENDERER!");
        }
        #endregion
    }
}
