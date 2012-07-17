using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Psynergy.Graphics
{
    public class DepthOfFieldProperties : AbstractPostProcessorProperties
    {
        private float m_BlurStart;
        private float m_BlurEnd;
        private float m_BlurAmount;

        public DepthOfFieldProperties() 
            : base()
        {
            m_BlurStart = 0;
            m_BlurEnd = 0;
            m_BlurAmount = 0;
        }

        public DepthOfFieldProperties(float start, float end) 
            : base(true)
        {
            m_BlurStart = start;
            m_BlurEnd = end;
            m_BlurAmount = 1;
        }

        public DepthOfFieldProperties(float start, float end, float blurAmount) : base(true)
        {
            m_BlurStart = start;
            m_BlurEnd = end;
            m_BlurAmount = blurAmount;
        }

        public float BlurStart { get { return m_BlurStart; } set { m_BlurStart = value; } }
        public float BlurEnd { get { return m_BlurEnd; } set { m_BlurEnd = value; } }
        public float BlurAmount { get { return m_BlurAmount; } set { m_BlurAmount = value; } }
    };
}
