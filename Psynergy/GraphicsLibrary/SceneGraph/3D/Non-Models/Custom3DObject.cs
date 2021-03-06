﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

/* Main Library */
using Psynergy;

/* Camera Library */
using Psynergy.Camera;

namespace Psynergy.Graphics
{
    public class Custom3DObject : Node3D
    {
        public Custom3DObject() : base("")
        {
        }

        public Custom3DObject(String name)
            : base(name)
        {
        }

        protected override void LoadSpecific()
        {
            base.LoadSpecific();

            // load textures
            LoadTextures();

            // Set up node vertices
            SetUpVertices();

            // Set up node indices
            SetUpIndices();

            // Run any post vertices / indices functions before copying the buffers
            SetUpSpecific();

            // copy vertex and index values to graphics card buffers
            CopyBuffers();
        }

        protected override void LoadTextures()
        {
            base.LoadTextures();

            if (m_Textures.Count > 0)
            {
                m_Textured = true;

                // Set current effect technique to be textured
                if (m_CurrentEffectTechnique == "")
                    m_CurrentEffectTechnique = "Textured";
            }
            else
            {
                // Set current effect technique to be textured
                if (m_CurrentEffectTechnique == "")
                    m_CurrentEffectTechnique = "NoTexture";
            }
        }

        protected virtual void SetUpVertices()
        {
        }

        protected virtual void SetUpIndices()
        {
        }

        protected virtual void SetUpSpecific()
        {
        }

        private void CopyBuffers()
        {
            // Copy vertex buffer
            CopyVertexBuffer();

            // Copy index buffers
            CopyIndexBuffers();
        }

        protected virtual void CopyVertexBuffer()
        {
            if (!m_Textured)
            {
                if (m_Vertices != null)
                {
                    m_VertexBuffer = new VertexBuffer(m_GraphicsDevice, VertexPositionColor.VertexDeclaration, m_Vertices.Length, BufferUsage.WriteOnly);
                    m_VertexBuffer.SetData(m_Vertices);
                }
            }
            else
            {
                if (m_TexturedVertices != null)
                {
                    m_VertexBuffer = new VertexBuffer(m_GraphicsDevice, VertexPositionNormalTexture.VertexDeclaration, m_TexturedVertices.Length, BufferUsage.WriteOnly);
                    m_VertexBuffer.SetData(m_TexturedVertices);
                }
            }
        }

        protected virtual void CopyIndexBuffers()
        {
            if (m_Indices != null)
            {
                m_IndexBuffer = new IndexBuffer(m_GraphicsDevice, typeof(int), m_Indices.Length, BufferUsage.WriteOnly);
                m_IndexBuffer.SetData(m_Indices);
            }
        }

        public override void Render(GameTime deltaTime)
        {
            base.Render(deltaTime);

            // Set the model cull mode
            /*RenderManager.Instance.SetCullMode(CullMode.None);

            if ((m_IndexBuffer != null) && (m_VertexBuffer != null))
            {
                m_GraphicsDevice.Indices = m_IndexBuffer;
                m_GraphicsDevice.SetVertexBuffer(m_VertexBuffer);

                if (m_CurrentEffect != null)
                {
                    foreach (EffectPass pass in m_CurrentEffect.CurrentTechnique.Passes)
                    {
                        pass.Apply();

                        //if (m_GraphicsDevice.SamplerStates[1] == null)
                            //m_GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;

                        // Render specifically for primitive drawing types
                        m_GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, GetVertexCount(), 0, GetPrimitiveCount());
                    }
                }

            }
            else
            {
                if (m_IndexBuffer == null)
                    Console.WriteLine("[Warning] - Index buffer is null on object " + Name + ".");

                if (m_VertexBuffer == null)
                    Console.WriteLine("[Warning] - Vertex buffer is null on object " + Name + ".");
            }

            RenderManager.Instance.SetCullMode(CullMode.CullCounterClockwiseFace);*/
        }
    }
}
