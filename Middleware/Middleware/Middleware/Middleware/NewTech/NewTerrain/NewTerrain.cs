using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Psynergy;
using Psynergy.Graphics;

namespace Middleware
{
    public class NewTerrain : Custom3DObject
    {
        #region Factory Property setting
        protected override void ClassProperties(Factory factory)
        {
            factory.RegisterString("heightmap", "HeightMap");

            base.ClassProperties(factory);
        }
        #endregion

        private String m_HeightMapName = "";    // Name of the height map files
        private Texture2D m_HeightMap = null;   // The height map used for the terrain.
        private IndexBuffer m_UnusedIndexBuffer = null;

        // QuadTree
        private QuadTree m_QuadTree = null;

        public NewTerrain() : base("")
        {
            int spug = 0;
        }

        public NewTerrain(String name) : base(name)
        {
            int spug = 0;
        }

        public override void Initialise()
        {
            base.Initialise();
        }

        public override void Load()
        {
            base.Load();

            // Fire a loaded event saying this terrain has been loaded
            //TerrainLoadedEvent terrainLoadedEvent = new TerrainLoadedEvent(this);
            //terrainLoadedEvent.Fire();
        }

        protected override void LoadSpecific()
        {
            // If a height map was set then load it 
            if (m_HeightMapName != "")
            {
                m_HeightMap = RenderManager.Instance.LoadTexture2D(m_HeightMapName);

                if (m_HeightMap != null)
                {
                    // Height map loaded so create quad vertex data
                    m_QuadTree = new QuadTree(this, m_HeightMap);
                   // m_TerrainInfo = new QuadTreeTerrainInfo(m_HeightMap);
                    
                    // Any extra information that is required...
                }
            }

            // Load any info after this
            base.LoadSpecific();
        }

        protected override void LoadTextures()
        {
            base.LoadTextures();
        }

        protected override void SetUpVertices()
        {
            if (!m_Textured)
            {
            }
            else
            {

                if ((m_QuadTree != null) && (m_QuadTree.TerrainInfo != null))
                {
                    m_TexturedVertices = new VertexPositionNormalTexture[m_QuadTree.TerrainInfo.VertexCount];
                    Color[] heightMapColours = new Color[m_QuadTree.TerrainInfo.VertexCount];

                    if (m_HeightMap != null)
                    {
                        // Get the heightmap data
                        m_HeightMap.GetData(heightMapColours);

                        float x = Position.X;
                        float y = Position.Y;
                        float z = Position.Z;
                        float maxX = (x + m_QuadTree.TerrainInfo.TopSize);

                        for (int i = 0; i < m_QuadTree.TerrainInfo.VertexCount; i++)
                        {
                            if (x > maxX)
                            {
                                // Reset row position
                                x = Position.X;
                                z++;
                            }

                            y = Position.Y + (heightMapColours[i].R * 0.2f);

                            // Create vertex
                            VertexPositionNormalTexture vert = new VertexPositionNormalTexture(new Vector3((x * ScaleX), (y * ScaleY), (z * ScaleZ)), Vector3.Zero, Vector2.Zero);
                            
                            // Work out UV mapping
                            vert.TextureCoordinate = new Vector2(((vert.Position.X - Position.X) / m_QuadTree.TerrainInfo.TopSize), ((vert.Position.Z - Position.Z) / m_QuadTree.TerrainInfo.TopSize));

                            // Save created vert
                            m_TexturedVertices[i] = vert;

                            // Increment the vertex count created
                            x++;
                        }

                    }

                }
            }

            // Now load the rest of the quadtree
            m_QuadTree.Load();
        }

        protected override void SetUpSpecific()
        {
            if ((m_QuadTree != null) && (m_QuadTree.TerrainInfo != null))
            {
                if (m_QuadTree.TerrainInfo.VertexCount < 9)
                    return;

                int i = (m_QuadTree.TerrainInfo.TopSize + 2);
                int j = 0;
                int k = (i + m_QuadTree.TerrainInfo.TopSize);

                for (int n = 0; (i <= ((m_QuadTree.TerrainInfo.VertexCount - m_QuadTree.TerrainInfo.TopSize) - 2)); i += 2, n++, j += 2, k += 2)
                {
                    if (n == m_QuadTree.TerrainInfo.HalfSize)
                    {
                        n = 0;
                        i += (m_QuadTree.TerrainInfo.TopSize + 2);
                        j += (m_QuadTree.TerrainInfo.TopSize + 2);
                        k += (m_QuadTree.TerrainInfo.TopSize + 2);
                    }

                    // Calculate normals for each 8 triangles
                    SetNormals(i, j, (j + 1));
                    SetNormals(i, (j + 1), (j + 2));
                    SetNormals(i, (j + 2), (i + 1));
                    SetNormals(i, (i + 1), (k + 2));
                    SetNormals(i, (k + 2), (k + 1));
                    SetNormals(i, (k + 1), k);
                    SetNormals(i, k, (i - 1));
                    SetNormals(i, (i - 1), j);
                }
            }
        }

        private void SetNormals(int index1, int index2, int index3)
        {
            if (m_Textured)
            {
                if (index3 > m_TexturedVertices.Length)
                    index3 = (m_TexturedVertices.Length - 1);

                Vector3 normal = Vector3.Cross((m_TexturedVertices[index2].Position - m_TexturedVertices[index1].Position), (m_TexturedVertices[index1].Position - m_TexturedVertices[index3].Position));
                normal.Normalize();

                // Add normals to correct indexed triangles
                m_TexturedVertices[index1].Normal += normal;
                m_TexturedVertices[index2].Normal += normal;
                m_TexturedVertices[index3].Normal += normal;
            }
        }

        protected override void CopyIndexBuffers()
        {
            // Limited to 100000 indices right now
            m_IndexBuffer = new IndexBuffer(m_GraphicsDevice, IndexElementSize.ThirtyTwoBits, 100000, BufferUsage.WriteOnly);
            m_UnusedIndexBuffer = new IndexBuffer(m_GraphicsDevice, IndexElementSize.ThirtyTwoBits, 100000, BufferUsage.WriteOnly);
        }

        public void UpdateIndexBuffer(int[] indices, int indexCount)
        {
            // Set indices to the buffer not being used
            m_UnusedIndexBuffer.SetData(indices, 0, indexCount);
        }

        public void SwapIndexBuffer()
        {
            IndexBuffer temp = m_IndexBuffer;

            // Swap the buffers
            m_IndexBuffer = m_UnusedIndexBuffer;
            m_UnusedIndexBuffer = temp;
        }

        public VertexPositionNormalTexture this[int index]
        {
            get { return m_TexturedVertices[index]; }
            set { m_TexturedVertices[index] = value; }
        }

        #region Set / Gets
        public String HeightMap { get { return m_HeightMapName; } set { m_HeightMapName = value; } }
        #endregion
    }
}
