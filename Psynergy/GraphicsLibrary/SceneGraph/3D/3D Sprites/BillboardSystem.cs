using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

/* Main Library */
using Psynergy;

/* Main Library */
using Psynergy.Camera;

namespace Psynergy.Graphics
{
    public class BillboardSystem : GameObject
    {
        #region Factory Property setting
        protected override void ClassProperties(Factory factory)
        {
            factory.RegisterVector2("billboardsize", "BillboardSize");

            base.ClassProperties(factory);
        }
        #endregion

        public enum BillboardMode { Cylindrical, Spherical };

        private VertexBuffer m_VertexBuffer;
        private IndexBuffer m_IndexBuffer;
        private VertexPositionTexture[] m_Particles;
        int[] m_Indices;

        private int m_NumBillboards = 0;
        private Vector2 m_BillboardSize = Vector2.Zero;
        private Texture2D m_Texture = null;

        // Graphics device and effect
        private GraphicsDevice m_GraphicsDevice;
        private Effect m_Effect;

        private bool m_EnsureOcclusion = true;
        public BillboardMode m_BillboardMode = BillboardMode.Spherical;

        public BillboardSystem() : base("")
        {
        }

        public BillboardSystem(GraphicsDevice graphicsDevice, ContentManager content, Texture2D texture, Vector2 billboardSize, Vector3[] particlePositions) : base("")
        {
            m_NumBillboards = particlePositions.Length;
            m_BillboardSize = billboardSize;
            m_GraphicsDevice = graphicsDevice;
            m_Texture = texture;
            m_Effect = content.Load<Effect>("Billboard");

            // Generate the billboard particles
            GenerateParticles(particlePositions);
        }

        public BillboardSystem(String name, GraphicsDevice graphicsDevice, ContentManager content, Texture2D texture, Vector2 billboardSize, Vector3[] particlePositions) : base(name)
        {
            m_NumBillboards = particlePositions.Length;
            m_BillboardSize = billboardSize;
            m_GraphicsDevice = graphicsDevice;
            m_Texture = texture;
            m_Effect = RenderManager.Instance.LoadEffect("Billboard");

            // Generate the billboard particles
            GenerateParticles(particlePositions);
        }

        public void GenerateParticles(Vector3[] particlePositions)
        {
            // Create vertex and index arrays
            m_Particles = new VertexPositionTexture[m_NumBillboards * 4];
            m_Indices = new int[m_NumBillboards * 6];
            int x = 0;

            // For each billboard
            for (int i = 0; i < (m_NumBillboards * 4); i+=4)
            {
                Vector3 pos = particlePositions[i / 4];

                // Add 4 vertices at the billboards position
                m_Particles[i + 0] = new VertexPositionTexture(pos, new Vector2(0, 0));
                m_Particles[i + 1] = new VertexPositionTexture(pos, new Vector2(0, 1));
                m_Particles[i + 2] = new VertexPositionTexture(pos, new Vector2(1, 1));
                m_Particles[i + 3] = new VertexPositionTexture(pos, new Vector2(1, 0));

                // Add 6 indices
                m_Indices[x++] = (i + 0);
                m_Indices[x++] = (i + 3);
                m_Indices[x++] = (i + 2);
                m_Indices[x++] = (i + 2);
                m_Indices[x++] = (i + 1);
                m_Indices[x++] = (i + 0);
            }

            // Create and set the vertex buffer
            m_VertexBuffer = new VertexBuffer(m_GraphicsDevice, VertexPositionTexture.VertexDeclaration, m_Particles.Length, BufferUsage.WriteOnly );
            m_VertexBuffer.SetData<VertexPositionTexture>(m_Particles);

            // Creates and sets the index buffer
            m_IndexBuffer = new IndexBuffer(m_GraphicsDevice, IndexElementSize.ThirtyTwoBits, (m_NumBillboards * 6), BufferUsage.WriteOnly);
            m_IndexBuffer.SetData<int>(m_Indices);
        }

        public override void Render(GameTime deltaTime)
        {
            base.Render(deltaTime);

            /*Camera3D gameCamera = (RenderManager.Instance.GetActiveCamera() as Camera3D);

            if (gameCamera != null)
            {
                // Set the vertex and index buffer to the graphics card
                m_GraphicsDevice.SetVertexBuffer(m_VertexBuffer);
                m_GraphicsDevice.Indices = m_IndexBuffer;

                // Set to alpha blend
                RenderManager.Instance.SetBlendState( BlendState.AlphaBlend );

                // Need the up and right vectors
                SetEffectParameters(gameCamera.View, gameCamera.Projection, gameCamera.Position, gameCamera.Up, gameCamera.Right);

                if (m_EnsureOcclusion)
                {
                    DrawOpaquePixels();
                    DrawTransparentPixels();
                }
                else
                {
                    RenderManager.Instance.SetDepthStencilState( DepthStencilState.DepthRead );

                    // No alpha test
                    m_Effect.Parameters["xAlphaTest"].SetValue(false);

                    // Draw the billboards
                    DrawBillboards();
                }

                // Reset to alpha blend
                RenderManager.Instance.SetBlendState( BlendState.Opaque );
                RenderManager.Instance.SetDepthStencilState( DepthStencilState.Default );

                // Unset the vertex and index buffer
                m_GraphicsDevice.SetVertexBuffer(null);
                m_GraphicsDevice.Indices = null;
            }*/
        }

        private void DrawOpaquePixels()
        {
            RenderManager.Instance.SetDepthStencilState( DepthStencilState.Default );

            m_Effect.Parameters["xAlphaTest"].SetValue(true);
            m_Effect.Parameters["xAlphaTestGreater"].SetValue(true);

            // Draw billboards
            DrawBillboards();
        }

        private void DrawTransparentPixels()
        {
            RenderManager.Instance.SetDepthStencilState( DepthStencilState.DepthRead );

            m_Effect.Parameters["xAlphaTest"].SetValue(true);
            m_Effect.Parameters["xAlphaTestGreater"].SetValue(false);

            // Draw billboards
            DrawBillboards();
        }

        private void DrawBillboards()
        {
            m_Effect.CurrentTechnique.Passes[0].Apply();

            // Draw the billboards
            m_GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, (4 * m_NumBillboards), 0, (m_NumBillboards * 2));
        }

        private void SetEffectParameters(Matrix view, Matrix projection, Vector3 camPos, Vector3 up, Vector3 right)
        {
            m_Effect.CurrentTechnique = m_Effect.Techniques["Technique1"];
            SetEffectParameter(m_Effect, "xParticleTexture", m_Texture);
            SetEffectParameter(m_Effect, "xWorld", Matrix.Identity);
            SetEffectParameter(m_Effect, "xView", view);
            SetEffectParameter(m_Effect, "xProjection", projection);
            SetEffectParameter(m_Effect, "xSize", (m_BillboardSize / 2.0f));
            SetEffectParameter(m_Effect, "xUp", m_BillboardMode == BillboardMode.Spherical ?  up : Vector3.Up);
            SetEffectParameter(m_Effect, "xSide", right);
            SetEffectParameter(m_Effect, "xCamPos", camPos);
            SetEffectParameter(m_Effect, "xAllowedRotDir", new Vector3( 0, 1, 0 ));

            // Apply changes
            m_Effect.CurrentTechnique.Passes[0].Apply();
        }

         /* Sets the specified effect parameter to the given effect, if it has that parameter */
        protected void SetEffectParameter(Effect effect, string paramName, object val)
        {
            // Debug.Assert(effect.Parameters[paramName] != null, "[Warning] - parameter '" + paramName + "' was not found on effect '" + effect.Name + "'.");

            if (effect.Parameters[paramName] == null)
                return;

            if (val is int)
                effect.Parameters[paramName].SetValue((int)val);
            if (val is float)
                effect.Parameters[paramName].SetValue((float)val);
            if (val is Vector2)
                effect.Parameters[paramName].SetValue((Vector2)val);
            if (val is Vector3)
                effect.Parameters[paramName].SetValue((Vector3)val);
            if (val is Vector4)
                effect.Parameters[paramName].SetValue((Vector4)val);
            else if (val is bool)
                effect.Parameters[paramName].SetValue((bool)val);
            else if (val is Matrix)
                effect.Parameters[paramName].SetValue((Matrix)val);
            else if (val is Matrix[])
                effect.Parameters[paramName].SetValue((Matrix[])val);
            else if (val is Texture)
                effect.Parameters[paramName].SetValue((Texture)val);
        }

        #region Set / Gets
        public Vector2 BillboardSize { get { return m_BillboardSize; } set { m_BillboardSize = value; } }
        public BillboardMode Mode { get { return m_BillboardMode; } set { m_BillboardMode = value; } }
        #endregion
    }
}
