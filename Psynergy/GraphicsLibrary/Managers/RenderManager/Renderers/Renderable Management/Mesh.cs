using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using SkinnedModel;

using Psynergy.Camera;

namespace Psynergy.Graphics
{
    public class Mesh : AbstractMesh
    {
        #region Variables
        /// <summary>
        /// Whether the mesh is active
        /// </summary>
        protected bool m_Active = true;

        /// <summary>
        /// Whether to ignore culling via the camera or not ( mainly use for sky boxes )
        /// </summary>
        protected bool m_IgnoreCameraCulling = false;

        /// <summary>
        /// Stores the transforms for the submeshes
        /// </summary>
        protected Matrix[] m_Transforms;

        /// <summary>
        /// our global transform
        /// </summary>
        protected Matrix m_Transform = Matrix.Identity;

        /// <summary>
        /// Model
        /// </summary>
        protected Model m_Model;
        protected MeshMetaData m_Metadata;
        protected BoundingBox m_GlobalBoundingBox;
        protected BoundingBox m_LocalBoundingBox;

        private List<SubMesh> m_SubMeshes = new List<SubMesh>();
        #endregion

        public Mesh()
        {
        }

        #region Sub Mesh Functions
        /// <summary>
        /// Propagate our global transform to the sub meshes, recomputing their bounding boxes
        /// </summary>
        private void UpdateSubMeshes()
        {
            Helpers.TransformBoundingBox(ref m_LocalBoundingBox, ref m_Transform, out m_GlobalBoundingBox);
            
            if (m_Model != null)
            {
                for (int i = 0; i < m_Model.Bones.Count; i++)
                {
                    m_Transforms[i] = m_Model.Bones[i].Transform * m_Transform;
                }

                for (int i = 0; i < m_SubMeshes.Count; i++)
                {
                    SubMesh subMesh = m_SubMeshes[i];
                   
                    //compute the global transform for this submesh
                    subMesh.GlobalTransform = m_Transforms[m_Model.Meshes[subMesh.ModelIndex].ParentBone.Index];
                    BoundingBox source = subMesh.MetaData.BoundingBox;
                   
                    //compute the global bounding box
                    Helpers.TransformBoundingBox(ref source, ref m_Transform, out subMesh.GlobalBoundingBox);
                }
            }
        }

        protected void CreateSubMeshList()
        {
            int idx = 0;

            for (int i = 0; i < m_Model.Meshes.Count; i++)
            {
                ModelMesh modelMesh = m_Model.Meshes[i];
                for (int index = 0; index < modelMesh.MeshParts.Count; index++)
                {
                    ModelMeshPart modelMeshPart = modelMesh.MeshParts[index];
                    SubMesh subMesh = CreateSubMesh(this);

                    subMesh.MeshPart = modelMeshPart;
                    subMesh.ModelIndex = idx;
                    subMesh.Effect = modelMeshPart.Effect;
                    
                    // Set submesh meta data
                    subMesh.MetaData = m_Metadata.SubMeshesMetadata[idx];
                    m_SubMeshes.Add(subMesh);
                }

                idx++;
            }

            UpdateSubMeshes();
        }

        protected virtual SubMesh CreateSubMesh(Mesh mesh)
        {
            return new SubMesh(mesh);
        }
        #endregion

        #region Setter Functions
        public override void SetTexture(Texture2D texture)
        {
            if (texture != null)
            {
                foreach (SubMesh subMesh in m_SubMeshes)
                    subMesh.SetTexture(texture);
            }
        }
        #endregion

        #region Property Set / Gets
        public bool Active
        {
            get { return m_Active; }
            set { m_Active = value; }
        }

        public bool IgnoreCameraCulling
        {
            get { return m_IgnoreCameraCulling; }
            set { m_IgnoreCameraCulling = value; }
        }

        public virtual Matrix[] BoneMatrices
        {
            get { return null; }
            set { }
        }

        public virtual Model Model
        {
            get { return m_Model; }
            set
            {
                m_SubMeshes.Clear();
                m_Model = value;
                m_Metadata = (m_Model.Tag as MeshMetaData);
                //the last bounding box is the sum of all others
                m_LocalBoundingBox = m_Metadata.BoundingBox;
                m_Transforms = new Matrix[m_Model.Bones.Count];
                m_Model.CopyAbsoluteBoneTransformsTo(m_Transforms);

                CreateSubMeshList();
                UpdateSubMeshes();
            }
        }

        public Matrix Transform
        {
            get { return m_Transform; }
            set
            {
                m_Transform = value;
                UpdateSubMeshes();
            }
        }

        public BoundingBox GlobalBoundingBox
        {
            get { return m_GlobalBoundingBox; }
        }

        public List<SubMesh> SubMeshes
        {
            get { return m_SubMeshes; }
        }

        public ERenderQueue RenderQueue
        {
            get { return m_Metadata.RenderQueue; }
        }

        public override bool CastShadows
        {
            get 
            {
                bool castShadows = base.CastShadows;

                foreach (SubMesh subMesh in m_SubMeshes)
                {
                    if (subMesh.CastShadows)
                    {
                        castShadows = true;
                        break;
                    }
                }

                return castShadows; 
            }
        }

        public override bool ReflectsObjects
        {
            get
            {
                bool reflectsObjects = base.ReflectsObjects;

                foreach (SubMesh subMesh in m_SubMeshes)
                {
                    if (subMesh.ReflectsObjects)
                    {
                        reflectsObjects = true;
                        break;
                    }
                }

                return reflectsObjects;
            }
        }
        #endregion
    }
}
