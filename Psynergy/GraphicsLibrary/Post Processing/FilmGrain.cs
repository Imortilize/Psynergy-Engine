﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Psynergy.Graphics
{

    public class FilmGrain : PostProcessor<FilmGrainProperties>
    {
        private Random m_RandomGenerator = new Random();

        public FilmGrain(GraphicsDevice graphicsDevice)
            : base(graphicsDevice, new FilmGrainProperties())
        {
        }

        public FilmGrain(GraphicsDevice graphicsDevice, Effect effect)
            : base(graphicsDevice, effect, new FilmGrainProperties())
        {
        }

        public override void Reset()
        {
            base.Reset();
        }

        public override void Draw(Texture2D texture)
        {
            // Depth of field reies on the face we have a graphics device, an effect assigned and 
            // a blur post processor to commit the depth of field against  
            if ((m_GraphicsDevice != null) && (Effect != null))
            {
                Effect.Parameters["xRandomValue"].SetValue((m_RandomGenerator.Next(1, 10000) / 100.0f));
                Effect.Parameters["xSceneTexture"].SetValue(texture);

                // Draw a quad to render the film grain
                base.Draw(null);
            }
        }

        protected override void SetSamplerStates()
        {
            base.SetSamplerStates();

            // Set sampler state
            m_GraphicsDevice.SamplerStates[9] = SamplerState.AnisotropicClamp;
        }

        public void SetProperties(FilmGrainProperties properties)
        {
            if (properties != null)
            {
                m_Properties = properties;

                if (Effect != null)
                {
                    Effect.Parameters["xFilmGrainStrength"].SetValue(m_Properties.FilmGrainStrength);
                    Effect.Parameters["xAccentuateDarkNoisePower"].SetValue(m_Properties.DarkNoisePower);
                    Effect.Parameters["xRandomNoiseStrength"].SetValue(m_Properties.RandomNoiseStrength);
                }
            }
        }

        public void SetGrainStrength(float grainStrength)
        {
            m_Properties.FilmGrainStrength = grainStrength;

            if (Effect != null)
                Effect.Parameters["xFilmGrainStrength"].SetValue(grainStrength);
        }

        public void SetDarkNoisePower(float darkNoisePower)
        {
            m_Properties.DarkNoisePower = darkNoisePower;

            if (Effect != null)
                Effect.Parameters["xAccentuateDarkNoisePower"].SetValue(darkNoisePower);
        }

        public void SetRandomNoiseStrength(float randomNoiseStrength)
        {
            m_Properties.RandomNoiseStrength = randomNoiseStrength;

            if (Effect != null)
                Effect.Parameters["xRandomNoiseStrength"].SetValue(randomNoiseStrength);
        }

        #region Setter Functions
        protected override void SetEffectParameters()
        {
            if (Effect != null)
            {
                Effect.Parameters["xFilmGrainStrength"].SetValue(m_Properties.FilmGrainStrength);
                Effect.Parameters["xAccentuateDarkNoisePower"].SetValue(m_Properties.DarkNoisePower);
                Effect.Parameters["xRandomNoiseStrength"].SetValue(m_Properties.RandomNoiseStrength);
            }
        }
        #endregion

        #region Set / Get
        #endregion
    }
}
