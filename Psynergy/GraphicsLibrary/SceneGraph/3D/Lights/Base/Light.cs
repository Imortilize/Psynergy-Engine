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

namespace Psynergy.Graphics 
{
    // TODO : Create a 'RenderNode' class which overloads Node3D overload to abstract general node stuff from the 3d render properties
    public class Light : Node
    {
        #region Factory Property setting
        protected override void ClassProperties(Factory factory)
        {
            factory.RegisterVector3("ambient", "AmbientColor");
            factory.RegisterVector3("diffuse", "DiffuseColor");
            factory.RegisterBool("allowinput", "InputAllowed");

            base.ClassProperties(factory);
        }
        #endregion

        #region Light Effect
        protected Effect m_Effect = null;
        #endregion

        protected Texture2D m_ColorMap = null;
        protected Texture2D m_NormalMap = null;
        protected Texture2D m_DepthMap = null;
        private Texture2D m_ShadowMap = null;

        protected bool m_InputAllowed = false;
        protected bool m_CastShadows = false;
        protected Vector2 m_HalfPixel = new Vector2(0, 0);

        public Light()
        {
            DiffuseColor = new Vector3((Color.White.R * 0.85f), (Color.White.G * 0.85f), (Color.White.B * 0.85f));
        }

        public override void Update(GameTime deltaTime)
        {
            base.Update(deltaTime);

            // Update material input ( debug purposes )
            UpdateInput(deltaTime);
        }

        protected virtual void UpdateInput(GameTime gameTime)
        {
        }

        public virtual void SetEffectParameters(Effect effect)
        {
            if (effect.Parameters["xLightColor"] != null)
                effect.Parameters["xLightColor"].SetValue(DiffuseColor / 255);
        }

        public override void Render(GameTime deltaTime)
        {
            base.Render(deltaTime);
        }

        public virtual void Draw(GameTime deltaTime)
        {
        }

        #region Property Set / Gets
        public Vector3 DiffuseColor { get; set; }
        public bool InputAllowed { get { return m_InputAllowed; } set { m_InputAllowed = value; } }
        public virtual bool CastShadow 
        {
            get { return m_CastShadows; }
            set
            {
                m_CastShadows = value;

                if (m_Effect != null)
                {
                    if ( m_Effect.Parameters["xEnableShadows"] != null )
                        m_Effect.Parameters["xEnableShadows"].SetValue(CastShadow);
                }
            }
        }

        public Matrix ViewProjection { get; set; }

        public virtual Texture2D ColorMap
        {
            get { return m_ColorMap; }
            set
            {
                m_ColorMap = value;

                if (m_Effect != null)
                    m_Effect.Parameters["xColorMap"].SetValue(value);
            }
        }

        public virtual Texture2D NormalMap
        {
            get { return m_NormalMap; }
            set
            {
                m_NormalMap = value;

                if (m_Effect != null)
                    m_Effect.Parameters["xNormalMap"].SetValue(value);
            }
        }

        public virtual Texture2D DepthMap
        {
            get { return m_DepthMap; }
            set
            {
                m_DepthMap = value;

                if (m_Effect != null)
                    m_Effect.Parameters["xDepthMap"].SetValue(value);
            }
        }

        public virtual Texture2D ShadowMap
        {
            get { return m_ShadowMap; }
            set
            {
                m_ShadowMap = value;

                if (m_Effect != null)
                {
                    if ( m_Effect.Parameters["xShadowMap"] != null )
                        m_Effect.Parameters["xShadowMap"].SetValue(value);

                    if (m_Effect.Parameters["xShadowMapSize"] != null)
                        m_Effect.Parameters["xShadowMapSize"].SetValue(new Vector2(value.Width, value.Height));
                }
            }
        }

        public virtual Vector2 HalfPixel
        {
            get { return m_HalfPixel; }
            set
            {
                m_HalfPixel = value;

                if (m_Effect != null)
                    m_Effect.Parameters["xHalfPixel"].SetValue(value);
            }
        }
        #endregion
    }
}
