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
        }

        #region Render Loop
        public override void Begin()
        {
            if (GraphicsDevice != null)
            {
                // Initial rendering state
                GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                GraphicsDevice.RasterizerState = RasterizerState.CullClockwise;
                GraphicsDevice.BlendState = BlendState.Opaque;
            }
        }

        public override void Draw(GameTime deltaTime)
        {
            if (GraphicsDevice != null)
            {
            
            }
        }

        public override void End()
        {
            base.End();
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
