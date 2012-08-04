using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Psynergy.Graphics;

namespace Psynergy.Graphics
{
    public class WaterMesh : Mesh
    {
        // Reference to Reflection buffer target texture 
        private RenderTarget2D m_ReflectionTarget = null;
        private RenderTarget2D m_RefractionTarget = null;
        private Texture2D m_OffsetMap = null;
        private Texture2D m_NormalMap = null;

        public WaterMesh(RenderTarget2D reflectionTarget, RenderTarget2D refractionTarget, Texture2D offsetMap, Texture2D normalMap) : base()
        {
            m_ReflectionTarget = reflectionTarget;
            m_RefractionTarget = refractionTarget;
            m_OffsetMap = offsetMap;
            m_NormalMap = normalMap;
        }

        protected override SubMesh CreateSubMesh(Mesh mesh)
        {
            return new WaterSubMesh(mesh, m_ReflectionTarget, m_RefractionTarget, m_OffsetMap, m_NormalMap);
        }
    }
}
