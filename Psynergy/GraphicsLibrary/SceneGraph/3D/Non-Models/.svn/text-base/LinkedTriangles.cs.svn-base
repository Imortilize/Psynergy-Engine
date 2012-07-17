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
    class LinkedTrianglesNode : Custom3DObject
    {
        public LinkedTrianglesNode() : base("")
        {
        }

        public LinkedTrianglesNode(String name) : base(name)
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
            m_Vertices = new VertexPositionColor[5];

            // Vertex 0
            m_Vertices[0].Position = new Vector3(0f, 0f, 0f);
            m_Vertices[0].Color = Color.Red;

            // Vertex 1
            m_Vertices[1].Position = new Vector3(1f, 0f, 0f);
            m_Vertices[1].Color = Color.Yellow;

            // Vertex 2
            m_Vertices[2].Position = new Vector3(2f, 0f, 0f);
            m_Vertices[2].Color = Color.Green;

            // Vertex 3
            m_Vertices[3].Position = new Vector3(1f, 1f, 0f);
            m_Vertices[3].Color = Color.Red;

            // Vertex 4
            m_Vertices[4].Position = new Vector3(2f, 1f, 0f);
            m_Vertices[4].Color = Color.Yellow;
        }

        protected override void SetUpIndices()
        {
            m_Indices = new int[6];

            m_Indices[0] = 3;
            m_Indices[1] = 1;
            m_Indices[2] = 0;
            m_Indices[3] = 4;
            m_Indices[4] = 2;
            m_Indices[5] = 1;
        }

        public override void Update(GameTime deltaTime)
        {
            base.Update(deltaTime);
        }
    }
}  
