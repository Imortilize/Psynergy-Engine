using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using Psynergy.Graphics;
using Psynergy.Camera;

namespace Psynergy.Graphics
{
    public class SkySphereSubMesh : SubMesh
    {
        public SkySphereSubMesh(Mesh mesh) : base(mesh)
        {
        }

        public override void ReconstructShading(GameTime deltaTime, Camera3D camera, Matrix view, Matrix projection, Microsoft.Xna.Framework.Graphics.GraphicsDevice graphicsDevice)
        {
            base.GenericRender(camera, graphicsDevice);
        }
        #region Effect Setters
        #endregion
    }
}
