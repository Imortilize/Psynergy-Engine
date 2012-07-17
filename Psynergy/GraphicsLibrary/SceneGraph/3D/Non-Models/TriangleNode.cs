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
    class TriangleNode : Custom3DObject
    {
        public TriangleNode() : base("")
        {
        }

        public TriangleNode(String name) : base(name)
        {
        }

        public override void Initialise()
        {
            base.Initialise();
        }

        public override void Load()
        {
            base.Load();
        }

        protected override void SetUpVertices()
        {
            if (!m_Textured)
            {
                m_Vertices = new VertexPositionColor[3];

                // Vertex 0
                m_Vertices[0].Position = new Vector3(0f, 0f, 0f);
                m_Vertices[0].Color = Color.Red;

                // Vertex 1
                m_Vertices[1].Position = new Vector3(0f, 1f, 0f);
                m_Vertices[1].Color = Color.Yellow;

                // Vertex 2
                m_Vertices[2].Position = new Vector3(1f, 0f, -1f);
                m_Vertices[2].Color = Color.Green;
            }
            else
            {
                m_TexturedVertices = new VertexPositionNormalTexture[3];

                // Vertex 0
                m_TexturedVertices[0].Position = new Vector3(0f, 0f, 0f);
                m_TexturedVertices[0].TextureCoordinate = new Vector2(0.0f, 0.0f);
                m_TexturedVertices[0].Normal = new Vector3(0.0f, 0.0f, 0.0f);

                // Vertex 1
                m_TexturedVertices[1].Position = new Vector3(0f, 1f, 0f);
                m_TexturedVertices[1].TextureCoordinate = new Vector2(1.0f, 0.0f);
                m_TexturedVertices[1].Normal = new Vector3(0.0f, 0.0f, 0.0f);

                // Vertex 2
                m_TexturedVertices[2].Position = new Vector3(1f, 0f, -1f);
                m_TexturedVertices[2].TextureCoordinate = new Vector2(0.0f, 1.0f);
                m_TexturedVertices[2].Normal = new Vector3(0.0f, 0.0f, 0.0f);
            }
        }

        protected override void SetUpIndices()
        {
            m_Indices = new int[3];

            // FRONT
            m_Indices[0] = 2;
            m_Indices[1] = 1;
            m_Indices[2] = 0;
        }

        public override void Update(GameTime deltaTime)
        {
            base.Update(deltaTime);
        }
    }
}
