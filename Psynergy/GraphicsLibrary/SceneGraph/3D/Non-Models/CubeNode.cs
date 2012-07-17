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
    public class CubeNode : Custom3DObject
    {
        protected BoundingBox m_BoundingBox = new BoundingBox();

        public CubeNode() : base("")
        {
        }

        public CubeNode(String name) : base(name)
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

        protected override void LoadSpecific()
        {
            base.LoadSpecific();
        }

        protected override void SetUpVertices()
        {
            if (!m_Textured)
            {
                m_Vertices = new VertexPositionColor[8];

                // Vertices for the cube
                m_Vertices[0].Position = new Vector3(-0.5f, -0.5f, -0.5f);
                m_Vertices[0].Color = Color.Green;
                m_Vertices[1].Position = new Vector3(0.5f, -0.5f, -0.5f);
                m_Vertices[1].Color = Color.Yellow;
                m_Vertices[2].Position = new Vector3(0.5f, 0.5f, -0.5f);
                m_Vertices[2].Color = Color.Red;
                m_Vertices[3].Position = new Vector3(-0.5f, 0.5f, -0.5f);
                m_Vertices[3].Color = Color.BlueViolet;
                m_Vertices[4].Position = new Vector3(-0.5f, -0.5f, 0.5f);
                m_Vertices[4].Color = Color.Red;
                m_Vertices[5].Position = new Vector3(-0.5f, 0.5f, 0.5f);
                m_Vertices[5].Color = Color.Yellow;
                m_Vertices[6].Position = new Vector3(0.5f, -0.5f, 0.5f);
                m_Vertices[6].Color = Color.Red;
                m_Vertices[7].Position = new Vector3(0.5f, 0.5f, 0.5f);
                m_Vertices[7].Color = Color.Green;
            }
            else
            {
                m_TexturedVertices = new VertexPositionNormalTexture[24];

                // Front - Done
                m_TexturedVertices[0].Position = new Vector3(-0.5f, -0.5f, -0.5f);
                m_TexturedVertices[0].TextureCoordinate = new Vector2(0.0f, 1.0f);
                m_TexturedVertices[0].Normal = new Vector3(0.0f, 0.0f, -1.0f);

                m_TexturedVertices[1].Position = new Vector3(0.5f, -0.5f, -0.5f);
                m_TexturedVertices[1].TextureCoordinate = new Vector2(1.0f, 1.0f);
                m_TexturedVertices[1].Normal = new Vector3(0.0f, 0.0f, -1.0f);

                m_TexturedVertices[2].Position = new Vector3(0.5f, 0.5f, -0.5f);
                m_TexturedVertices[2].TextureCoordinate = new Vector2(1.0f, 0.0f);
                m_TexturedVertices[2].Normal = new Vector3(0.0f, 0.0f, -1.0f);

                m_TexturedVertices[3].Position = new Vector3(-0.5f, 0.5f, -0.5f);
                m_TexturedVertices[3].TextureCoordinate = new Vector2(0.0f, 0.0f);
                m_TexturedVertices[3].Normal = new Vector3(0.0f, 0.0f, -1.0f);

                // LEFT - Done
                m_TexturedVertices[4].Position = new Vector3(-0.5f, -0.5f, 0.5f);
                m_TexturedVertices[4].TextureCoordinate = new Vector2(0.0f, 1.0f);
                m_TexturedVertices[4].Normal = new Vector3(-1.0f, 0.0f, 0.0f);

                m_TexturedVertices[5].Position = new Vector3(-0.5f, -0.5f, -0.5f);
                m_TexturedVertices[5].TextureCoordinate = new Vector2(1.0f, 1.0f);
                m_TexturedVertices[5].Normal = new Vector3(-1.0f, 0.0f, 0.0f);

                m_TexturedVertices[6].Position = new Vector3(-0.5f, 0.5f, -0.5f);
                m_TexturedVertices[6].TextureCoordinate = new Vector2(1.0f, 0.0f);
                m_TexturedVertices[6].Normal = new Vector3(-1.0f, 0.0f, 0.0f);

                m_TexturedVertices[7].Position = new Vector3(-0.5f, 0.5f, 0.5f);
                m_TexturedVertices[7].TextureCoordinate = new Vector2(0.0f, 0.0f);
                m_TexturedVertices[7].Normal = new Vector3(-1.0f, 0.0f, 0.0f);

                // RIGHT - Done
                m_TexturedVertices[8].Position = new Vector3(0.5f, -0.5f, -0.5f);
                m_TexturedVertices[8].TextureCoordinate = new Vector2(0.0f, 1.0f);
                m_TexturedVertices[8].Normal = new Vector3(1.0f, 0.0f, 0.0f);

                m_TexturedVertices[9].Position = new Vector3(0.5f, -0.5f, 0.5f);
                m_TexturedVertices[9].TextureCoordinate = new Vector2(1.0f, 1.0f);
                m_TexturedVertices[9].Normal = new Vector3(1.0f, 0.0f, 0.0f);

                m_TexturedVertices[10].Position = new Vector3(0.5f, 0.5f, 0.5f);
                m_TexturedVertices[10].TextureCoordinate = new Vector2(1.0f, 0.0f);
                m_TexturedVertices[10].Normal = new Vector3(1.0f, 0.0f, 0.0f);

                m_TexturedVertices[11].Position = new Vector3(0.5f, 0.5f, -0.5f);
                m_TexturedVertices[11].TextureCoordinate = new Vector2(0.0f, 0.0f);
                m_TexturedVertices[11].Normal = new Vector3(1.0f, 0.0f, 0.0f);

                // BACK - Done
                m_TexturedVertices[12].Position = new Vector3(0.5f, -0.5f, 0.5f);
                m_TexturedVertices[12].TextureCoordinate = new Vector2(0.0f, 1.0f);
                m_TexturedVertices[12].Normal = new Vector3(0.0f, 0.0f, 1.0f);

                m_TexturedVertices[13].Position = new Vector3(-0.5f, -0.5f, 0.5f);
                m_TexturedVertices[13].TextureCoordinate = new Vector2(1.0f, 1.0f);
                m_TexturedVertices[13].Normal = new Vector3(0.0f, 0.0f, 1.0f);

                m_TexturedVertices[14].Position = new Vector3(-0.5f, 0.5f, 0.5f);
                m_TexturedVertices[14].TextureCoordinate = new Vector2(1.0f, 0.0f);
                m_TexturedVertices[14].Normal = new Vector3(0.0f, 0.0f, 1.0f);

                m_TexturedVertices[15].Position = new Vector3(0.5f, 0.5f, 0.5f);
                m_TexturedVertices[15].TextureCoordinate = new Vector2(0.0f, 0.0f);
                m_TexturedVertices[15].Normal = new Vector3(0.0f, 0.0f, 1.0f);

                // TOP - Done
                m_TexturedVertices[16].Position = new Vector3(-0.5f, 0.5f, -0.5f);
                m_TexturedVertices[16].TextureCoordinate = new Vector2(0.0f, 1.0f);
                m_TexturedVertices[16].Normal = new Vector3(0.0f, 1.0f, 0.0f);

                m_TexturedVertices[17].Position = new Vector3(0.5f, 0.5f, -0.5f);
                m_TexturedVertices[17].TextureCoordinate = new Vector2(1.0f, 1.0f);
                m_TexturedVertices[17].Normal = new Vector3(0.0f, 1.0f, 0.0f);

                m_TexturedVertices[18].Position = new Vector3(0.5f, 0.5f, 0.5f);
                m_TexturedVertices[18].TextureCoordinate = new Vector2(1.0f, 0.0f);
                m_TexturedVertices[18].Normal = new Vector3(0.0f, 1.0f, 0.0f);

                m_TexturedVertices[19].Position = new Vector3(-0.5f, 0.5f, 0.5f);
                m_TexturedVertices[19].TextureCoordinate = new Vector2(0.0f, 0.0f);
                m_TexturedVertices[19].Normal = new Vector3(0.0f, 1.0f, 0.0f);

                // BOTTOM
                m_TexturedVertices[20].Position = new Vector3(-0.5f, -0.5f, 0.5f);
                m_TexturedVertices[20].TextureCoordinate = new Vector2(0.0f, 1.0f);
                m_TexturedVertices[20].Normal = new Vector3(0.0f, -1.0f, 0.0f);

                m_TexturedVertices[21].Position = new Vector3(0.5f, -0.5f, 0.5f);
                m_TexturedVertices[21].TextureCoordinate = new Vector2(1.0f, 1.0f);
                m_TexturedVertices[21].Normal = new Vector3(0.0f, -1.0f, 0.0f);

                m_TexturedVertices[22].Position = new Vector3(0.5f, -0.5f, -0.5f);
                m_TexturedVertices[22].TextureCoordinate = new Vector2(1.0f, 0.0f);
                m_TexturedVertices[22].Normal = new Vector3(0.0f, -1.0f, 0.0f);

                m_TexturedVertices[23].Position = new Vector3(-0.5f, -0.5f, -0.5f);
                m_TexturedVertices[23].TextureCoordinate = new Vector2(0.0f, 0.0f);
                m_TexturedVertices[23].Normal = new Vector3(0.0f, -1.0f, 0.0f);
            }
        }

        protected override void SetUpIndices()
        {
            m_Indices = new int[36];

            if (!m_Textured)
            {
                // FRONT
                m_Indices[0] = 2;
                m_Indices[1] = 1;
                m_Indices[2] = 0;
                m_Indices[3] = 0;
                m_Indices[4] = 3;
                m_Indices[5] = 2;

                // LEFT
                m_Indices[6] = 3;
                m_Indices[7] = 0;
                m_Indices[8] = 4;
                m_Indices[9] = 4;
                m_Indices[10] = 5;
                m_Indices[11] = 3;

                // RIGHT
                m_Indices[12] = 7;
                m_Indices[13] = 6;
                m_Indices[14] = 1;
                m_Indices[15] = 1;
                m_Indices[16] = 2;
                m_Indices[17] = 7;

                // BACK
                m_Indices[18] = 5;
                m_Indices[19] = 4;
                m_Indices[20] = 6;
                m_Indices[21] = 6;
                m_Indices[22] = 7;
                m_Indices[23] = 5;

                // TOP
                m_Indices[24] = 7;
                m_Indices[25] = 2;
                m_Indices[26] = 3;
                m_Indices[27] = 3;
                m_Indices[28] = 5;
                m_Indices[29] = 7;

                // BOTTOM
                m_Indices[30] = 0;
                m_Indices[31] = 1;
                m_Indices[32] = 6;
                m_Indices[33] = 6;
                m_Indices[34] = 4;
                m_Indices[35] = 0;
            }
            else
            {
                // FRONT
                m_Indices[0] = 0;
                m_Indices[1] = 1;
                m_Indices[2] = 2;
                m_Indices[3] = 2;
                m_Indices[4] = 3;
                m_Indices[5] = 0;

                // LEFT
                m_Indices[6] = 6;
                m_Indices[7] = 5;
                m_Indices[8] = 4;
                m_Indices[9] = 4;
                m_Indices[10] = 7;
                m_Indices[11] = 6;

                // RIGHT
                m_Indices[12] = 10;
                m_Indices[13] = 9;
                m_Indices[14] = 8;
                m_Indices[15] = 8;
                m_Indices[16] = 11;
                m_Indices[17] = 10;

                // BACK
                m_Indices[18] = 14;
                m_Indices[19] = 13;
                m_Indices[20] = 12;
                m_Indices[21] = 12;
                m_Indices[22] = 15;
                m_Indices[23] = 14;

                // TOP
                m_Indices[24] = 18;
                m_Indices[25] = 17;
                m_Indices[26] = 16;
                m_Indices[27] = 16;
                m_Indices[28] = 19;
                m_Indices[29] = 18;

                // BOTTOM
                m_Indices[30] = 22;
                m_Indices[31] = 21;
                m_Indices[32] = 20;
                m_Indices[33] = 20;
                m_Indices[34] = 23;
                m_Indices[35] = 22;
            }
        }

        public override void Update(GameTime deltaTime)
        {
            base.Update(deltaTime);

            // Update bounding box
            m_BoundingBox.Min = Position - new Vector3((0.5f * Scale.X), (0.5f * Scale.Y), (0.5f * Scale.Z));
            m_BoundingBox.Max = Position + new Vector3((0.5f * Scale.X), (0.5f * Scale.Y), (0.5f * Scale.Z));
        }

        public override void Render(GameTime deltaTime)
        {
            base.Render(deltaTime);
        }

        #region Property Set / Gets
        public BoundingBox BoundingBox { get { return m_BoundingBox; } }
        #endregion
    }
}
