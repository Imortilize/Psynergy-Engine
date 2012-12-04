using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using SkinnedModel;

using Psynergy.Camera;

namespace Psynergy.Graphics
{
    public class SubMesh : AbstractMesh
    {
        #region Variables
        private MeshMetaData.SubMeshMetadata m_Metadata;
        private ModelMeshPart m_MeshPart;
        private int m_ModelIndex = 0;

        private Matrix m_GlobalTransform = Matrix.Identity;
        public BoundingBox GlobalBoundingBox;
        private BaseRenderEffect m_RenderEffect;
        private Mesh m_Parent;
        #endregion

        public SubMesh(Mesh parent)
        {
            m_Parent = parent;
        }

        #region Render Functions
        public override void RenderToGBuffer(Camera3D camera, GraphicsDevice graphicsDevice)
        {
            RenderEffect.SetCurrentTechnique(0);
            RenderEffect.SetMatrices(GlobalTransform, camera.Transform, camera.Projection);

            //our first pass is responsible for rendering into GBuffer
            RenderEffect.SetFarClip(camera.FarPlane);

            if (m_Parent.BoneMatrices != null)
                RenderEffect.SetBones(m_Parent.BoneMatrices);

            RenderEffect.Apply();

            graphicsDevice.SetVertexBuffer(m_MeshPart.VertexBuffer, m_MeshPart.VertexOffset);
            graphicsDevice.Indices = m_MeshPart.IndexBuffer;

            // Draw indexed primitives for this sub mesh
            graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, m_MeshPart.NumVertices, m_MeshPart.StartIndex, m_MeshPart.PrimitiveCount);
        }

        public override void ReconstructShading(GameTime deltaTime, Camera3D camera, Matrix view, Matrix projection, GraphicsDevice graphicsDevice)
        {
            // this pass uses the light accumulation texture and reconstruct the mesh's shading
            // our parameters were already filled in the first pass
            RenderEffect.SetCurrentTechnique(1);
            RenderEffect.SetMatrices(GlobalTransform, view, projection);

            // Apply the Render Effect
            RenderEffect.Apply();

            graphicsDevice.SetVertexBuffer(m_MeshPart.VertexBuffer, m_MeshPart.VertexOffset);
            graphicsDevice.Indices = m_MeshPart.IndexBuffer;

            graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, m_MeshPart.NumVertices, m_MeshPart.StartIndex, m_MeshPart.PrimitiveCount);
        }

        public override void GenericRender(Camera3D camera, GraphicsDevice graphicsDevice)
        {
            RenderEffect.SetMatrices(GlobalTransform, camera.Transform, camera.Projection);

            if (m_Parent.BoneMatrices != null)
                RenderEffect.SetBones(m_Parent.BoneMatrices);

            // Set camera position
            RenderEffect.SetCameraPosition(camera.Position);

            // Apply the Render Effect
            RenderEffect.Apply();

            // Switch cull mode 
            RenderManager.Instance.SetCullMode(CullMode.CullClockwiseFace);
            RenderManager.Instance.SetDepthStencilState(DepthStencilState.DepthRead);

            graphicsDevice.SetVertexBuffer(m_MeshPart.VertexBuffer, m_MeshPart.VertexOffset);
            graphicsDevice.Indices = m_MeshPart.IndexBuffer;

            graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, m_MeshPart.NumVertices, m_MeshPart.StartIndex, m_MeshPart.PrimitiveCount);

            // Switch cull mode 
            RenderManager.Instance.SetDepthStencilState(DepthStencilState.Default);
            RenderManager.Instance.SetCullMode(CullMode.CullCounterClockwiseFace);
        }

        public override void RenderShadowMap(ref Matrix viewProj, GraphicsDevice graphicsDevice)
        {
            //render to shadow map
            RenderEffect.SetCurrentTechnique(2);
            RenderEffect.SetLightViewProj(viewProj);

            // Apply Render effect
            RenderEffect.Apply();

            // Set shadow cull mode
            RenderManager.Instance.SetCullMode(ShadowCullMode);

            graphicsDevice.SetVertexBuffer(m_MeshPart.VertexBuffer, m_MeshPart.VertexOffset);
            graphicsDevice.Indices = m_MeshPart.IndexBuffer;

            graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, m_MeshPart.NumVertices, m_MeshPart.StartIndex, m_MeshPart.PrimitiveCount);

            // Set shadow cull mode
            RenderManager.Instance.SetCullMode(CullMode.CullCounterClockwiseFace);
        }
        #endregion

        #region Setter Functions
        public override void SetTexture(Texture2D texture)
        {
            if ((texture != null) && (m_RenderEffect != null))
                m_RenderEffect.SetColorBuffer(texture);
        }

        protected virtual void OnSetEffect(BaseRenderEffect effect)
        {
        }
        #endregion

        #region Set / Gets
        public Effect Effect
        {
            set
            {
                m_RenderEffect = new BaseRenderEffect(value.Clone());

                // Call any render effect setters for this sub mesh
                if (m_RenderEffect != null)
                    OnSetEffect(m_RenderEffect);
            }
        }

        public MeshMetaData.SubMeshMetadata MetaData
        {
            get { return m_Metadata; }
            set { m_Metadata = value; }
        }

        public ModelMeshPart MeshPart
        {
            get { return m_MeshPart; }
            set { m_MeshPart = value; }
        }

        public Matrix GlobalTransform
        {
            get { return m_GlobalTransform; }
            set { m_GlobalTransform = value; }
        }

        public int ModelIndex
        {
            get { return m_ModelIndex; }
            set { m_ModelIndex = value; }
        }

        public ERenderQueue RenderQueue
        {
            get { return m_Metadata.RenderQueue; }
        }

        public BaseRenderEffect RenderEffect
        {
            get { return m_RenderEffect; }
        }

        public CullMode ShadowCullMode
        {
            get { return m_Metadata.ShadowCullMode; }
        }

        #region Properties
        public override bool CastShadows
        {
            get { return ((m_Metadata.CastShadows) && (m_Metadata.RenderQueue != ERenderQueue.Blend)); }
        }

        public override bool ReflectsObjects
        {
            get { return ((m_Metadata.ReflectsObjects) && (m_Metadata.RenderQueue == ERenderQueue.Reflects)); }
        }
        #endregion
        #endregion
    }
}
