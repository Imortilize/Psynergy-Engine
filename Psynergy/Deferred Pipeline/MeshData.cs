using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using SkinnedModel;

namespace DeferredPipeline
{
    public class MeshData
    {
        #region Skinning
        private SkinningData m_SkinningData;
        #endregion

        #region Textures
        private String m_TextureFilename = "";
        #endregion

        #region Effects
        private Effect m_Effect = null;
        #endregion

        public MeshData()
        {
        }

        #region Property Set / Gets
        public SkinningData SkinningData
        {
            get { return m_SkinningData; }
            set { m_SkinningData = value; }
        }

        public String TextureFilename
        {
            get { return m_TextureFilename; }
            set { m_TextureFilename = value; }
        }

        public Effect Effect
        {
            get { return m_Effect; }
            set { m_Effect = value; }
        }
        #endregion
    }
}
