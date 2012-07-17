using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Psynergy.Graphics
{
    public class FogProperties : AbstractPostProcessorProperties
    {
        private Color m_Color = Color.White;
        private Vector3 m_EffectColor = Vector3.Zero;
        private float m_Start = 0.0f;
        private float m_Distance = 0.0f;
        private float m_End = 0.0f;

        public FogProperties()
            : base()
        {
            m_Color = Color.White;
            m_EffectColor = new Vector3(m_Color.R, m_Color.G, m_Color.B);
            m_Start = 0;
            m_Distance = 0;
            m_End = (m_Start + m_Distance);
        }

        public FogProperties(Color color, float start, float distance) : base(true)
        {
            m_Color = color;
            m_EffectColor = new Vector3(m_Color.R, m_Color.G, m_Color.B);
            m_Start = start;
            m_Distance = distance;
            m_End = (m_Start + m_Distance);
        }

        public Color FogColor { get { return m_Color; } set { m_Color = value; } }
        public Vector3 EffectColor { get { return m_EffectColor; } set { m_EffectColor = value; } }
        public float Start { get { return m_Start; } set { m_Start = value; } }
        public float Distance { get { return m_Distance; } set { m_Distance = value; } }
        public float End { get { return m_End; } set { m_End = value; } }
    }
}
