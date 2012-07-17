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

namespace Psynergy.Graphics
{
    class MultiLight : Light
    {
        public Vector3[] LightDirection { get; set; }
        public Vector3[] LightColor { get; set; }

        public MultiLight()
        {
            LightDirection = new Vector3[3];
            LightColor = new Vector3[] { Vector3.One, Vector3.One, Vector3.One };
        }

        public override void Update(GameTime deltaTime)
        {
            base.Update(deltaTime);
        }

        public override void SetEffectParameters(Effect effect)
        {
            base.SetEffectParameters(effect);

            if (effect.Parameters["xLightColor"] != null)
                effect.Parameters["xLightColor"].SetValue(LightColor);

            if (effect.Parameters["xLightDirection"] != null)
                effect.Parameters["xLightDirection"].SetValue(LightDirection);
        }
    }
}
