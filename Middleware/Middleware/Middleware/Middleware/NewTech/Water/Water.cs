using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using SkinnedModel;
using Psynergy.Graphics;
using Psynergy.WaterPipeline;

namespace Middleware
{
    public class Water : ModelNode
    {
        private WaterInfo m_WaterInfo = null;

        // Reflection and Refraction buffer target texture 
        private RenderTarget2D m_ReflectionTarget = null;
        private RenderTarget2D m_RefractionTarget = null;

        // Water DUDV map
        private Texture2D m_OffsetMap = null;
        private Texture2D m_NormalMap = null;

        public Water() : base("")
        {
        }

        public Water(String name) : base(name)
        {
        }

        public override void Initialise()
        {
            // Cube specific variables
            //TextureName = "Textures/Others/default";
            //RenderGroupName = "shadow";

            // Set modelName
            ModelName = "Textures/water";
            //Scale *= 0.1f;

            // Initialise the reflection target
            GraphicsDevice graphicsDevice = RenderManager.Instance.GraphicsDevice;
            Vector2 screenSize = RenderManager.Instance.ActiveRenderer.ScreenSize;
            m_ReflectionTarget = new RenderTarget2D(graphicsDevice, (int)(screenSize.X * 1f), (int)(screenSize.Y * 1f), true, SurfaceFormat.Color, DepthFormat.Depth24);
            m_RefractionTarget = new RenderTarget2D(graphicsDevice, (int)(screenSize.X * 1f), (int)(screenSize.Y * 1f), true, SurfaceFormat.Color, DepthFormat.Depth24);

            base.Initialise();
        }

        protected override void LoadSpecific()
        {
            // Load Offset map
            m_OffsetMap = RenderManager.Instance.LoadTexture2D("Textures/WaterDUDV");
            m_NormalMap = RenderManager.Instance.LoadTexture2D("Textures/WaterNormal");

            base.LoadSpecific();

            // State that this should ignore camera culling 
            // ( stops sky spheres dissapearing due to camera not binding with it )
            if (m_Mesh != null)
                m_Mesh.IgnoreCameraCulling = true;
        }

        protected override Mesh CreateMesh()
        {
            Mesh mesh = new WaterMesh(m_ReflectionTarget, m_RefractionTarget, m_OffsetMap, m_NormalMap);

            // Create mesh data
            MeshMetaData meta = new MeshMetaData();
            meta.BoundingBox = m_BoundingBox;

            WaterMeshMetaData waterMeta = new WaterMeshMetaData();
            waterMeta.RenderQueue = ERenderQueue.Reflects;
            m_WaterInfo = waterMeta.WaterInfo = (m_Model.Tag as WaterInfo);
            waterMeta.EnableLighting = true;
            waterMeta.BoundingBox = m_BoundingBox;
            waterMeta.CastShadows = false;
            waterMeta.ReflectsObjects = true;

            // Add as sub mesh
            meta.AddSubMeshMetadata(waterMeta);

            // Save meta data
            m_Model.Tag = meta;

            // Get correct render effect
            foreach (ModelMesh modelMesh in m_Model.Meshes)
            {
                foreach (ModelMeshPart modelMeshPart in modelMesh.MeshParts)
                {
                    modelMeshPart.Effect = RenderManager.Instance.GetEffect("WaterGBuffer");

                    if (modelMeshPart.Effect != null)
                    {
                        if (m_Textures.Count > 0)
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

        public override void Update(GameTime deltaTime)
        {
            base.Update(deltaTime);

            float spritescaler = 0.2f;

            DebugRender.Instance.AddTexture2D(m_ReflectionTarget, new Vector2(100, 0), spritescaler);
            DebugRender.Instance.AddTexture2D(m_RefractionTarget, new Vector2(100 + (m_ReflectionTarget.Width * spritescaler), 0), spritescaler);
        }

        #region Property Set / Gets
        public WaterInfo TerrainInfo { get { return m_WaterInfo; } }
        #endregion
    }
}
