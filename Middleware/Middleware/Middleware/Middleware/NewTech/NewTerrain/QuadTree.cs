using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Psynergy.Camera;

namespace Middleware
{
    public class QuadTree
    {
        private QuadNode m_RootNode = null;
        private int m_TopNodeSize = 0;

        private Vector3 m_CameraPosition = Vector3.Zero;
        private Vector3 m_LastCameraPosition = Vector3.Zero;

        private int[] m_Indices;

        private GraphicsDevice m_GraphicsDevice = null;
        private NewTerrain m_Terrain = null;   
        private QuadTreeTerrainInfo m_TerrainInfo = null;

        public QuadTree(NewTerrain terrain, Texture2D heightMap)
        {
            m_Terrain = terrain;
            m_TerrainInfo = new QuadTreeTerrainInfo(heightMap);

            if (heightMap != null)
            {
                m_TopNodeSize = (heightMap.Width - 1);

                // Construct an array large enough to hold all of the indices we'll need
                m_Indices = new int[((heightMap.Width + 1) * (heightMap.Height + 1)) * 3];
            }
        }

        public void Load()
        {
            m_RootNode = new QuadNode(QuadNodeType.FullNode, m_TopNodeSize, 1, null, this, 0);

            if (CameraManager.Instance.ActiveCamera != null)
            {
                Camera3D camera3D = (CameraManager.Instance.ActiveCamera as Camera3D);

                if (camera3D != null)
                    ViewFrustum = new BoundingFrustum(camera3D.ViewProjection);
            }
        }


        #region Property Set / Gets
        public int TopNodeSize { get { return m_TopNodeSize; } }
        public QuadNode RootNode { get { return m_RootNode; } }
        public VertexPositionNormalTexture this[int index]
        {
            get 
            {
                return m_Terrain[index]; 
            }
            set 
            {
                if (m_Terrain != null)
                    m_Terrain[index] = value; 
            }
        }

        public QuadTreeTerrainInfo TerrainInfo { get { return m_TerrainInfo; } }
        public Vector3 CameraPosition { get { return m_CameraPosition; } }

        internal BoundingFrustum ViewFrustum { get; set; }
        #endregion
    }
}
