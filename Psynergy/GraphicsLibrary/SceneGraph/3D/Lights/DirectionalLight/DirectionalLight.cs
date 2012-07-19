using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

/* Main Library */
using Psynergy;
using Psynergy.Camera;

namespace Psynergy.Graphics
{
    public class DirectionalLight : Light, IRegister<DirectionalLight>
    {
        #region Factory Property setting
        protected override void ClassProperties(Factory factory)
        {
            factory.RegisterVector3("direction", "LightDirection");

            base.ClassProperties(factory);
        }
        #endregion

        private QuadRenderer m_QuadRenderer = new QuadRenderer();
        public Vector3 LightDirection { get; set; }

        public DirectionalLight()
        {
            LightDirection = new Vector3(1, 1, 1);
            DiffuseColor = new Vector3(1, 1, 1);
        }

        public override void Initialise()
        {
            base.Initialise();

            m_Effect = RenderManager.Instance.GetEffect("DirectionalLight").Clone();
            m_Effect.Parameters["xLightDirection"].SetValue(LightDirection);
            m_Effect.Parameters["xLightColor"].SetValue((DiffuseColor / 255));
        }

        public override void Load()
        {
            base.Load();
        }

        public override void SetEffectParameters(Effect effect)
        {
            base.SetEffectParameters(effect);

            if (effect.Parameters["xLightPosition"] != null)
                effect.Parameters["xLightPosition"].SetValue(Position);

            if (effect.Parameters["xLightDirection"] != null)
                effect.Parameters["xLightDirection"].SetValue(LightDirection);
        }

        public override void Render(GameTime deltaTime)
        {
            base.Render(deltaTime);
        }

        public override void Draw(GameTime deltaTime)
        {
            base.Draw(deltaTime);

            if (m_Effect != null)
            {
                Camera3D camera = (CameraManager.Instance.ActiveCamera as Camera3D);

                if (camera != null)
                {
                    m_Effect.Parameters["xCameraPosition"].SetValue(camera.Position);
                    m_Effect.Parameters["xCameraTransform"].SetValue(camera.Transform);
                    m_Effect.Parameters["xInvertViewProjection"].SetValue(Matrix.Invert(camera.ViewProjection));
                }

                m_Effect.Parameters["xLightViewProjection"].SetValue(ViewProjection);

                // Apply 
                m_Effect.CurrentTechnique.Passes[0].Apply();

                // Draw a full screen quad
                m_QuadRenderer.RenderQuad(RenderManager.Instance.GraphicsDevice, -Vector2.One, Vector2.One);
            }
        }

        #region Property Set / Gets
        #endregion
    }
}
