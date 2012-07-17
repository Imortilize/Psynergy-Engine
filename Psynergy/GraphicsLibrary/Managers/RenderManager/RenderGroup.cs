using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Psynergy.Graphics
{
    public class RenderGroup
    {
        private bool n_Active = false;
        private String m_Name = "";

        // Rendergroup renderqueries are in here
        private bool m_EnableLighting = true;
        private bool m_EnableTexture = true;
        private bool m_EnableShadow = true;
        private bool m_EnableFog = true;
        private bool m_Skinned = false;

        public RenderGroup(String name)
        {
            Debug.Assert(name != "");

            m_Name = name;
        }

        public bool Active { get { return n_Active; } set { n_Active = value; } }
        public String Name { get { return m_Name; } }

        public bool EnableLighting { get { return m_EnableLighting; } set { m_EnableLighting = value; } }
        public bool EnableTexture { get { return m_EnableTexture; } set { m_EnableTexture = value; } }
        public bool EnableShadow { get { return m_EnableShadow; } set { m_EnableShadow = value; } }
        public bool EnableFog { get { return m_EnableFog; } set { m_EnableFog = value; } }
        public bool IsSkinned { get { return m_Skinned; } set { m_Skinned = value; } }
    }
}
