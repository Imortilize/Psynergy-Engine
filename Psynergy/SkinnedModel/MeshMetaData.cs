//-----------------------------------------------------------------------------
// MeshMetadata.cs
//
// Jorge Adriano Luna 2011
// http://jcoluna.wordpress.com
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkinnedModel;

namespace SkinnedModel
{
    public enum ERenderQueue
    {
        Default,
        SkipGbuffer,
        Blend,
        Reflects,
        Count
    }

    /// <summary>
    /// This class holds information about a processed mesh
    /// </summary>
    public class MeshMetaData
    {
        private ERenderQueue m_RenderQueue = ERenderQueue.Default;

        public class SubMeshMetadata
        {
            private ERenderQueue m_RenderQueue = ERenderQueue.Default;
            private BoundingBox m_BoundingBox;
            private bool m_EnableLighting = false;
            private bool m_UseNormalMap = false;
            private bool m_CastShadows = false;
            private bool m_ReflectsObjects = false;
            private CullMode m_ShadowCullMode = CullMode.CullCounterClockwiseFace;

            public BoundingBox BoundingBox
            {
                get { return m_BoundingBox; }
                set { m_BoundingBox = value; }
            }

            public bool EnableLighting
            {
                get { return m_EnableLighting; }
                set { m_EnableLighting = value; }
            }

            public bool UseNormalMap
            {
                get { return m_UseNormalMap; }
                set { m_UseNormalMap = value; }
            }

            public bool CastShadows
            {
                get { return m_CastShadows; }
                set { m_CastShadows = value; }
            }

            public bool ReflectsObjects
            {
                get { return m_ReflectsObjects; }
                set { m_ReflectsObjects = value; }
            }

            public ERenderQueue RenderQueue
            {
                get { return m_RenderQueue; }
                set { m_RenderQueue = value; }
            }

            public CullMode ShadowCullMode
            {
                get { return m_ShadowCullMode; }
                set { m_ShadowCullMode = value; }
            }
        } 
        
        private List<SubMeshMetadata> m_SubMeshesMetadata = new List<SubMeshMetadata>();
        private SkinningData m_SkinningData;
        private BoundingBox m_BoundingBox;
        
        public void AddSubMeshMetadata(SubMeshMetadata metadata)
        {
            m_SubMeshesMetadata.Add(metadata);
        }

        public BoundingBox BoundingBox
        {
            get { return m_BoundingBox; }
            set { m_BoundingBox = value; }
        }

        public SkinningData SkinningData
        {
            get { return m_SkinningData; }
            set { m_SkinningData = value; }
        }

        public List<SubMeshMetadata> SubMeshesMetadata
        {
            get { return m_SubMeshesMetadata; }
        }

        public ERenderQueue RenderQueue
        {
            get { return m_RenderQueue; }
            set { m_RenderQueue = value; }
        }
    }
}
