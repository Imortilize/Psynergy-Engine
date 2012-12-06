using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Psynergy;
using Psynergy.Camera;

using SkinnedModel;

namespace Psynergy.Graphics
{
    class SkySphere : ModelNode, IRegister<SkySphere>
    {
        #region Factory Property setting
        protected override void ClassProperties(Factory factory)
        {
            factory.RegisterFloat("skysize", "SkySize");
            factory.RegisterString("skytexture", "SkyTextureName");

            base.ClassProperties(factory);
        }

        private float m_SkySize = 0.0f;
        private String m_SkyTextureName = "";
        private TextureCube m_Test = null;

        #endregion

        public SkySphere() : base("")
        {
        }

        public SkySphere(String name) : base(name)
        {
        }

        public override void Initialise()
        {
            base.Initialise();
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
            Mesh mesh = new SkySphereMesh();

            if (m_Model.Tag == null)
            {
                // Get cubemap effect
                Effect cubeMapEffect = RenderManager.Instance.GetEffect("cubemap");

                // Add Model meta data by default for now ( can't get skybox to compile with content processor atm )
                MeshMetaData metaData = new MeshMetaData();
                metaData.RenderQueue = ERenderQueue.SkipGbuffer;

                for (int i = 0; i < m_Model.Meshes.Count; i++)
                {
                    ModelMesh modelMesh = m_Model.Meshes[i];

                    for (int index = 0; index < modelMesh.MeshParts.Count; index++)
                    {
                        MeshMetaData.SubMeshMetadata subMeshData = new MeshMetaData.SubMeshMetadata();
                        subMeshData.RenderQueue = ERenderQueue.SkipGbuffer;

                        // Add Sub mesh data
                        metaData.AddSubMeshMetadata(subMeshData);

                        // Set cubemap effect
                        SetEffectParameter(cubeMapEffect, "Texture", m_Textures[0]);
                        modelMesh.MeshParts[index].Effect = cubeMapEffect;
                    }
                }

                m_Model.Tag = metaData;
            }

            return mesh;
        }


        protected override void LoadTextures()
        {
            if ( m_Model != null )
            {
                Texture[] textures = new Texture2D[m_Model.Meshes.Count];

                foreach (ModelMesh mesh in m_Model.Meshes)
                    foreach (BasicEffect currentEffect in mesh.Effects)
                    {
                        if ( currentEffect.Texture != null )
                            m_Textures.Add(currentEffect.Texture);
                    }

                if (m_SkyTextureName != "")
                {
                    m_Test = RenderManager.Instance.LoadTextureCube(m_SkyTextureName);
                    m_Textures.Add(m_Test); 
                }
            }
        }

        public override void Update(GameTime deltaTime)
        {
            base.Update(deltaTime);
        }

        public override void Render(GameTime deltaTime)
        {
            // Render model appropiately
            base.Render(deltaTime);
        }

        private void SetClipPlane(Vector4? plane)
        {
            if (m_CurrentEffect != null)
            {
                SetEffectParameter(m_CurrentEffect, "xClipPlaneEnabled", plane.HasValue);

                if (plane.HasValue)
                    SetEffectParameter(m_CurrentEffect, "xClipPlane", plane.Value);
            }
        }

        #region Set / Gets
        public float SkySize
        {
            get { return m_SkySize; }
            set
            {
                // Save sky size
                m_SkySize = value; 
                
                // Set scale
                transform.Scale = new Vector3(value);
            }
        }

        public String SkyTextureName { get { return m_SkyTextureName; } set { m_SkyTextureName = value; } }
        #endregion
    }
}
