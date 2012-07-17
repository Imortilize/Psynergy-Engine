using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Psynergy.Camera;

namespace Psynergy.Graphics
{
    public class AbstractMesh
    {
        public AbstractMesh()
        {
        }

        public virtual void Reset()
        {
        }

        public virtual void RenderToGBuffer(Camera3D camera, GraphicsDevice graphicsDevice)
        {
        }

        public virtual void ReconstructShading(GameTime deltaTime, Camera3D camera, Matrix view, Matrix projection, GraphicsDevice graphicsDevice)
        {
        }

        public virtual void GenericRender(Camera3D camera, GraphicsDevice graphicsDevice)
        {
        }

        public virtual void ReflectionRender(GameTime deltaTime, List<AbstractMesh> meshes, Camera3D camera, GraphicsDevice graphicsDevice)
        {
        }

        public virtual void RenderShadowMap(ref Matrix viewProj, GraphicsDevice graphicsDevice)
        {
        }

        #region Setter Functions
        public virtual void SetTexture(Texture2D texture)
        {
        }
        #endregion

        #region Properties
        public virtual bool CastShadows
        {
            get { return false; }
        }

        public virtual bool ReflectsObjects
        {
            get { return false; }
        }
        #endregion
    }
}