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
    class QuadRenderer
    {
        private VertexPositionTexture[] m_QuadVertices;
        private short[] m_QuadIndexBuffer = null;

        public QuadRenderer()
        {
            // Easier to use this seperately and not overload it
            Initialise();
        }

        private void Initialise()
        {
            m_QuadVertices = new VertexPositionTexture[4];

            // Vertices for the cube
            m_QuadVertices[0].Position = new Vector3(-1, 1, 0);
            m_QuadVertices[0].TextureCoordinate = new Vector2(0, 0);
            m_QuadVertices[1].Position = new Vector3(1, 1, 0);
            m_QuadVertices[1].TextureCoordinate = new Vector2(1, 0);
            m_QuadVertices[2].Position = new Vector3(-1, -1, 0);
            m_QuadVertices[2].TextureCoordinate = new Vector2(0, 1);
            m_QuadVertices[3].Position = new Vector3(1, -1, 0);
            m_QuadVertices[3].TextureCoordinate = new Vector2(1, 1);

            m_QuadIndexBuffer = new short[] { 0, 3, 2, 0, 1, 3 };
        }

        public void RenderQuad(GraphicsDevice graphicsDevice, Vector2 v1, Vector2 v2)
        {
            m_QuadVertices[0].Position.X = v1.X;
            m_QuadVertices[0].Position.Y = v2.Y;

            m_QuadVertices[1].Position.X = v2.X;
            m_QuadVertices[1].Position.Y = v2.Y;

            m_QuadVertices[2].Position.X = v1.X;
            m_QuadVertices[2].Position.Y = v1.Y;

            m_QuadVertices[3].Position.X = v2.X;
            m_QuadVertices[3].Position.Y = v1.Y;

            graphicsDevice.DrawUserIndexedPrimitives<VertexPositionTexture>
                (PrimitiveType.TriangleList, m_QuadVertices, 0, 4, m_QuadIndexBuffer, 0, 2);
        }
    }
}
