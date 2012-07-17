using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Psynergy.Graphics;

namespace Psynergy.Graphics
{
    public class SkySphereMesh : Mesh
    {
        public SkySphereMesh(): base()
        {

        }

        protected override SubMesh CreateSubMesh(Mesh mesh)
        {
            return new SkySphereSubMesh(mesh);
        }
    }
}
