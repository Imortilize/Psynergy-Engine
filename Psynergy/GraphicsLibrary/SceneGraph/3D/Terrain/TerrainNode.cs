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

namespace Psynergy.Graphics
{
    public class TerrainNode : Custom3DObject
    {
        #region Factory Property setting
        protected override void ClassProperties(Factory factory)
        {
            factory.RegisterString("heightmap", "HeightMap");
            factory.RegisterFloat("cellsize", "CellSize");
            factory.RegisterFloat("maxheight", "MaxHeight");
            factory.RegisterInt("width", "Width");
            factory.RegisterInt("length", "Length");

            base.ClassProperties(factory);
        }
        #endregion

        // Terrain specific variables
        private float[,] m_Heights;             // Heights store for terrain
        private float m_Height = 0;             // Maximum height of the terrain
        private float m_CellSize = 0;           // Distance between vertices on the X and Z axes.
        private int m_Width = 0;                // Number of vertices on the X axis
        private int m_Length = 0;               // Number of vertices on the Z axis
        private int m_NumVertices = 0;          // Number of vertices in the terrain
        private int m_NumIndices = 0;           // Number of indices in the terrain
        private String m_HeightMapName = "";    // Name of the height map files
        private Texture2D m_HeightMap = null;   // The height map used for the terrain.
        private float m_TextureTiling = 10;      // Tiling for texture mapping

        // Detail map variables
        private float m_DetailDistance = 2500;          // distance for the detail texture mapping
        private float m_DetailTextureTiling = 100;      // Tiling for detail texture mapping

        // Billboard system
        private BillboardSystem m_Trees;

        // New variables
        private float m_HeightMapWidth = 0.0f;
        private float m_HeightMapHeight = 0.0f;
        private Vector3 m_HeightMapPosition = Vector3.Zero;

        public TerrainNode() : base("")
        {
        }

        public TerrainNode(String name) : base(name)
        {
        }

        public override void Initialise()
        {
            base.Initialise();
        }

        public override void Load()
        {
            base.Load();

            // Fire a loaded event saying this terrain has been loaded
            TerrainLoadedEvent terrainLoadedEvent = new TerrainLoadedEvent(this);
            terrainLoadedEvent.Fire();
        }

        protected override void LoadSpecific()
        {
            // If a height map was set then load it 
            if (m_HeightMapName != "")
            {
                m_HeightMap = RenderManager.Instance.LoadTexture2D(m_HeightMapName);
                m_Width = m_HeightMap.Width;
                m_Length = m_HeightMap.Height;
            }

            // Calculate the number of vertices and indices
            m_NumVertices = (m_Width * m_Length);
            m_NumIndices = (m_Width - 1) * (m_Length - 1) * 6;

            // Get the height map heights
            GetHeights();

            // Set up the other terrain variables
            m_HeightMapWidth = (m_Heights.GetLength(0) - 1) * m_CellSize;
            m_HeightMapHeight = (m_Heights.GetLength(1) - 1) * m_CellSize;
            m_HeightMapPosition.X = -(m_Heights.GetLength(0) - 1) / 2.0f * m_CellSize;
            m_HeightMapPosition.Z = -(m_Heights.GetLength(1) - 1) / 2.0f * m_CellSize;

            // load base variables now the vertices have been set up
            base.LoadSpecific();

            // Set effect parameters up front if they can be
            if (m_CurrentEffect != null)
            {
                SetEffectParameter(m_CurrentEffect, "xRTexture", m_Textures[1]);
                SetEffectParameter(m_CurrentEffect, "xGTexture", m_Textures[2]);
                SetEffectParameter(m_CurrentEffect, "xBTexture", m_Textures[3]);
                SetEffectParameter(m_CurrentEffect, "xWeightTexture", m_Textures[4]);
                SetEffectParameter(m_CurrentEffect, "xDetailTexture", m_Textures[5]);
                SetEffectParameter(m_CurrentEffect, "xDetailTextureTiling", m_DetailTextureTiling);
                SetEffectParameter(m_CurrentEffect, "xDetailDistance", m_DetailDistance);
                SetEffectParameter(m_CurrentEffect, "TextureTiling", m_TextureTiling);
            }
        }

        private void GetHeights()
        {
            Debug.Assert(m_HeightMap != null, "Height map should not be null!");

            if (m_HeightMap != null)
            {
                // Extract pixel data
                Color[] heightMapData = new Color[m_Width * m_Length];

                // Get the height map data
                m_HeightMap.GetData<Color>(heightMapData);

                // Create heights[,] array
                m_Heights = new float[m_Width, m_Length];

                // For each pixel
                for (int y = 0; y < m_Length; y++)
                {
                    for (int x = 0; x < m_Width; x++)
                    {
                        // Get colour value ( 0 - 255 )
                        float color = heightMapData[y * m_Width + x].R;

                        // Scale to ( 0 - 1 )
                        color /= 255;

                        // Multiply by the max height to get the final height
                        m_Heights[x, y] = (color * m_Height);
                    }
                }  
            }
        }


        protected override void SetUpVertices()
        {
            if ( !m_Textured )
            {
            }
            else
            {
                m_TexturedVertices = new VertexPositionNormalTexture[m_NumVertices];

                // Calculate the position offset that will center the terrain at (0, 0, 0)
                Vector3 offsetToCenter = -new Vector3((((float)m_Width / 2.0f) * m_CellSize), 0, (((float)m_Length / 2.0f) * m_CellSize));

                // For each pixel in the image
                for (int z = 0; z < m_Length; z++)
                {
                    for (int x = 0; x < m_Width; x++)
                    {
                        // Find position based on grid coordinates and height in heightmap
                        Vector3 position = new Vector3((x * m_CellSize), m_Heights[x, z], (z * m_CellSize)) + offsetToCenter;

                        // UV coordinates range from (0, 0) at grid locations (0, 0) to 
                        // (1, 1) at grid locations (width, length)
                        Vector2 uv = new Vector2(((float)x / m_Width), ((float)z / m_Length));

                        // Create the vertex
                        m_TexturedVertices[z * m_Width + x] = new VertexPositionNormalTexture(position, Vector3.Zero, uv);
                    }
                }
            }
        }

        protected override void SetUpIndices()
        {
            m_Indices = new int[m_NumIndices];

            int i = 0;

            // For each cell
            for (int x = 0; x < (m_Width - 1); x++)
            {
                for (int z = 0; z < (m_Length - 1); z++)
                {
                    // Find the indices of the corners
                    int upperLeft = (z * m_Width + x);
                    int upperRight = (upperLeft + 1);
                    int lowerLeft = (upperLeft + m_Width);
                    int lowerRight = (lowerLeft + 1);

                    // Specify upper triangles
                    m_Indices[i++] = upperLeft;
                    m_Indices[i++] = upperRight;
                    m_Indices[i++] = lowerLeft;

                    // Specify lower triangles
                    m_Indices[i++] = lowerLeft;
                    m_Indices[i++] = upperRight;
                    m_Indices[i++] = lowerRight;
                }
            }
        }

        protected override void SetUpSpecific()
        {
            // For each triangle
            for (int i = 0; i < m_NumIndices; i+=3)
            {
                // Find the position of each corner of the tringle
                Vector3 v1 = Vector3.Zero; 
                Vector3 v2 = Vector3.Zero; 
                Vector3 v3 = Vector3.Zero;

                // Cross the vectors between the corners to get the normal
                Vector3 normal = Vector3.Zero; 

                if ( !m_Textured )
                {
                    // TODO?:
                }
                else
                {
                    v1 = m_TexturedVertices[m_Indices[i]].Position;
                    v2 = m_TexturedVertices[m_Indices[i + 1]].Position;
                    v3 = m_TexturedVertices[m_Indices[i + 2]].Position;

                    normal = -Vector3.Cross((v1 - v2), (v1 - v3));
                    normal.Normalize();

                    // Add the influence of the normal to each vertex in the triangle
                    m_TexturedVertices[m_Indices[i]].Normal += normal;
                    m_TexturedVertices[m_Indices[i + 1]].Normal += normal;
                    m_TexturedVertices[m_Indices[i + 2]].Normal += normal;
                }
            }

            // Average the influences of the triangles touching each vertex
            for (int i = 0; i < m_NumVertices; i++)
            {
                if (!m_Textured)
                {
                    // TODO?:
                }
                else
                    m_TexturedVertices[i].Normal.Normalize();
            }

            // Set up the billboard trees
            SetUpBillBoardTrees();
        }

        private void SetUpBillBoardTrees()
        {
            // Initialise the trees billboard system
            Random r = new Random();
            Vector3[] positions = new Vector3[100];

            for (int i = 0; i < positions.Length; i++)
            {
                positions[i] = new Vector3((float)r.NextDouble() * 5000 - 2500, 40, (float)r.NextDouble() * 5000 - 2500);

                // Cap to be on the terrain surface
                positions[i].Y = (GetHeight(positions[i]) + 45);
            }

            m_Trees = new BillboardSystem((m_Name + "-Trees"), RenderManager.Instance.GraphicsDevice, RenderManager.Instance.ContentManager, RenderManager.Instance.LoadTexture2D("Textures/Terrain/tree"), new Vector2(100), positions);

            m_Trees.Mode = BillboardSystem.BillboardMode.Cylindrical;
        }

        public override void Update(GameTime deltaTime)
        {
            base.Update(deltaTime);
        }

        public override void Render(GameTime deltaTime)
        {
            base.Render(deltaTime);

            // Is the proper render effect so render the billboards now
            /*if ( m_CachedEffect == null )
            {
                if (m_Trees != null)
                {
                    // Render the trees
                    m_Trees.Render(deltaTime);
                }
            }*/
        }

        public bool IsOnHeightMap(Vector3 position)
        {
            Vector3 positionOnHeightMap = (position - m_HeightMapPosition);

            // Check to see if that value goes outside the bounds of the heightmap
            return ((positionOnHeightMap.X > 0) && (positionOnHeightMap.X < m_HeightMapWidth) &&
                     (positionOnHeightMap.Z > 0) && (positionOnHeightMap.Z < m_HeightMapHeight));
        }

        public float GetHeight(Vector3 position)
        {
            float height = 0.0f;

            // Check it is on the height map
            if (IsOnHeightMap(position))
            {
                Vector3 positionOnHeightMap = (position - m_HeightMapPosition);

                int left = ((int)positionOnHeightMap.X / (int)m_CellSize);
                int top = ((int)positionOnHeightMap.Z / (int)m_CellSize);

                float xNormalized = (positionOnHeightMap.X % m_CellSize) / m_CellSize;
                float zNormalized = (positionOnHeightMap.Z % m_CellSize) / m_CellSize;

                float topHeight = MathHelper.Lerp(
                    m_Heights[left, top],
                    m_Heights[(left + 1), top],
                    xNormalized);

                float bottomHeight = MathHelper.Lerp(
                    m_Heights[left, (top + 1)],
                    m_Heights[(left + 1), (top + 1)],
                    xNormalized);

                // Next interpolate between these values to calculate the actual position
                height = MathHelper.Lerp(topHeight, bottomHeight, zNormalized);
            }

            return height;
        }

        public float GetHeight(Vector3 position, out float steepness)
        {
            float height = 0.0f;

            Vector3 positionOnHeightMap = (position - m_HeightMapPosition);

            int left = ((int)positionOnHeightMap.X / (int)m_CellSize);
            int top = ((int)positionOnHeightMap.Z / (int)m_CellSize);

            float xNormalized = (positionOnHeightMap.X % m_CellSize) / m_CellSize;
            float zNormalized = (positionOnHeightMap.Z % m_CellSize) / m_CellSize;

            float topHeight = MathHelper.Lerp(
                m_Heights[left, top],
                m_Heights[(left + 1), top],
                xNormalized);

            float bottomHeight = MathHelper.Lerp(
                m_Heights[left, (top + 1)],
                m_Heights[(left + 1), (top + 1)],
                xNormalized);

            // Next interpolate between these values to calculate the actual position
            height = MathHelper.Lerp(topHeight, bottomHeight, zNormalized);

            // Determine steepness ( angle between higher and lower vertex of cell )
            steepness = (float)Math.Atan(Math.Abs((topHeight - bottomHeight)) / (m_CellSize * Math.Sqrt(2)));

            return height;
        }

        #region Set / Gets
        public String HeightMap { get { return m_HeightMapName; } set { m_HeightMapName = value; } }
        public float CellSize { get { return m_CellSize; } set { m_CellSize = value; } }
        public int Width { get { return m_Width; } set { m_Width = value; } }
        public int Length { get { return m_Length; } set { m_Length = value; } }
        public float MaxHeight { get { return m_Height; } set { m_Height = value; } }
        #endregion
    }
}
