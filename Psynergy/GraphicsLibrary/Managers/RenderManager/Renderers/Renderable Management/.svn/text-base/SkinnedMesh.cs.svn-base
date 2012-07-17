using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using SkinnedModel;

namespace Psynergy.Graphics
{
    public class SkinnedMesh : Mesh
    {
        private Matrix[] m_BoneMatrices;
        private SkinningData m_SkinningData;

        public override Model Model
        {
            set
            {
                base.Model = value;
                MeshMetaData metadata = m_Model.Tag as MeshMetaData;
                m_SkinningData = metadata.SkinningData;
                m_BoneMatrices = m_SkinningData.BindPose.ToArray();
            }
        }

        public SkinningData SkinningData
        {
            get { return m_SkinningData; }
        }

        public override Matrix[] BoneMatrices
        {
            get { return m_BoneMatrices; }
            set { m_BoneMatrices = value; }
        }
    }
}
