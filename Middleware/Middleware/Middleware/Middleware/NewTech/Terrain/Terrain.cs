using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using SkinnedModel;
using Psynergy.TerrainPipeline;

using Psynergy.Graphics;

namespace Middleware
{
    public class Terrain : ModelNode
    {
        private TerrainInfo m_TerrainInfo = null;

        public Terrain() : base("")
        {
        }

        public Terrain(String name) : base(name)
        {
        }

        public override void Initialise()
        {
            // Cube specific variables
            //TextureName = "Textures/Others/default";
            //RenderGroupName = "shadow";

            // Set modelName
            ModelName = "Textures/HeightMaps/terrain";
            //Scale *= 0.1f;

            base.Initialise();
        }

        public override void  Load()
        {
 	         base.Load();

            // Fire a loaded event saying this terrain has been loaded
            TerrainLoadedEvent terrainLoadedEvent = new TerrainLoadedEvent(this);
            terrainLoadedEvent.Fire();
        }

        protected override void LoadSpecific()
        {
            base.LoadSpecific();

            // State that this should ignore camera culling 
            // ( stops sky spheres dissapearing due to camera not binding with it )
            if (m_Mesh != null)
                m_Mesh.IgnoreCameraCulling = true;
        }

        protected override Mesh CreateMesh()
        {
            Mesh mesh = base.CreateMesh();

            // Create mesh data
            MeshMetaData meta = new MeshMetaData();
            meta.BoundingBox = m_BoundingBox;

            TerrainMeshMetaData terrainMeta = new TerrainMeshMetaData();
            m_TerrainInfo = terrainMeta.TerrainInfo = (m_Model.Tag as TerrainInfo);
            terrainMeta.EnableLighting = true;
            terrainMeta.BoundingBox = m_BoundingBox;

            // Add as sub mesh
            meta.AddSubMeshMetadata(terrainMeta);

            // Save meta data
            m_Model.Tag = meta;

            // Get correct render effect
            foreach ( ModelMesh modelMesh in m_Model.Meshes )
            {
                foreach (ModelMeshPart modelMeshPart in modelMesh.MeshParts)
                {
                    modelMeshPart.Effect = RenderManager.Instance.GetEffect("RenderGBuffer");

                    if (modelMeshPart.Effect != null)
                    {
                        if ( m_Textures.Count > 0 )
                            modelMeshPart.Effect.Parameters["Texture"].SetValue(m_Textures[0]);
                    }
                }
            }

            // Create a metameshdata tag
            return mesh;
        }

        public override void Reset()
        {
            base.Reset();
        }

        #region Property Set / Gets
        public TerrainInfo TerrainInfo { get { return m_TerrainInfo; } }
        #endregion
    }
}
