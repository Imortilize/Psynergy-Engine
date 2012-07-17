using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

/* Input Library */
using Psynergy.Input;

namespace Psynergy.Graphics
{
    class SpotLight : Light
    {
        public Vector3 LightPosition { get; set; }
        public Vector3 LightColor { get; set; }
        public Vector3 LightDirection { get; set; }
        public float ConeAngle { get; set; }
        public float LightFalloff { get; set; }

        public SpotLight()
        {
            LightPosition = new Vector3(0, 3000, 0);
            LightColor = new Vector3( 0.85f, 0.85f, 0.85f );
            ConeAngle = 30;
            LightDirection = new Vector3(0, -1, 0);
            LightFalloff = 20;
        }

        public override void Update(GameTime deltaTime)
        {
            base.Update(deltaTime);

            // Update material input ( debug purposes )
            UpdateInput(deltaTime);
        }

        protected override void UpdateInput(GameTime gameTime)
        {
            InputManager input = InputManager.Instance;

            if (input.KeyDown(Keys.W))
                LightPosition += new Vector3(0, 10, 0);

            if (input.KeyDown(Keys.S))
                LightPosition -= new Vector3(0, 10, 0);

            if (input.KeyDown(Keys.D))
                LightPosition += new Vector3(10, 0, 0);

            if (input.KeyDown(Keys.A))
                LightPosition -= new Vector3(10, 0, 0);

            if (input.KeyDown(Keys.Right))
                ConeAngle += 0.1f;

            if (input.KeyDown(Keys.Left))
                ConeAngle -= 0.1f;
        }

        public override void SetEffectParameters(Effect effect)
        {
            base.SetEffectParameters(effect);

            if (effect.Parameters["xLightPosition"] != null)
                effect.Parameters["xLightPosition"].SetValue(LightPosition);

            if (effect.Parameters["xLightColor"] != null)
                effect.Parameters["xLightColor"].SetValue(LightColor);

            if (effect.Parameters["xLightDirection"] != null)
                effect.Parameters["xLightDirection"].SetValue(LightDirection);

            if (effect.Parameters["xConeAngle"] != null)
                effect.Parameters["xConeAngle"].SetValue(ConeAngle);

            if (effect.Parameters["xLightFallOff"] != null)
                effect.Parameters["xLightFallOff"].SetValue(LightFalloff);
        }
    }
}
