using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Psynergy.Graphics
{
    public class BloomProperties : AbstractPostProcessorProperties
    {
        public float m_BloomThreshold;
        public float m_BloomIntensity;
        public float m_OriginalIntensity;
        public float m_BloomSaturation;
        public float m_OriginalSaturation;

        public BloomProperties() 
            : base()
        {
            m_BloomThreshold = 0.3f;
            m_BloomIntensity = 1.3f;
            m_OriginalIntensity = 1.0f;
            m_BloomSaturation = 1.0f;
            m_OriginalSaturation = 1.0f;
        }

        public BloomProperties(float bloomIntensity) 
            : base(true)
        {
            m_BloomThreshold = 0.3f;
            m_BloomIntensity = bloomIntensity;
            m_OriginalIntensity = 1.0f;
            m_BloomSaturation = 1.0f;
            m_OriginalSaturation = 1.0f;
        }

        public BloomProperties(float bloomIntensity, float bloomSaturation)
            : base(true)
        {
            m_BloomThreshold = 0.3f;
            m_BloomIntensity = bloomIntensity;
            m_OriginalIntensity = 1.0f;
            m_BloomSaturation = bloomSaturation;
            m_OriginalSaturation = 1.0f;
        }

        public BloomProperties(float bloomIntensity, float bloomSaturation, float originalIntesity, float originalSaturation)
            : base(true)
        {
            m_BloomThreshold = 0.3f;
            m_BloomIntensity = bloomIntensity;
            m_OriginalIntensity = originalIntesity;
            m_BloomSaturation = bloomSaturation;
            m_OriginalSaturation = originalSaturation;
        }

        public BloomProperties(float bloomThreshold, float bloomIntensity, float bloomSaturation, float originalIntesity, float originalSaturation)
            : base(true)
        {
            m_BloomThreshold = bloomThreshold;
            m_BloomIntensity = bloomIntensity;
            m_OriginalIntensity = originalIntesity;
            m_BloomSaturation = bloomSaturation;
            m_OriginalSaturation = originalSaturation;
        }

        public float BloomThreshold { get { return m_BloomThreshold; } set { m_BloomThreshold = value; } }
        public float BloomIntensity { get { return m_BloomIntensity; } set { m_BloomIntensity = value; } }
        public float OriginalIntensity { get { return m_OriginalIntensity; } set { m_OriginalIntensity = value; } }
        public float BloomSaturation { get { return m_BloomSaturation; } set { m_BloomSaturation = value; } }
        public float OriginalSaturation { get { return m_OriginalSaturation; } set { m_OriginalSaturation = value; } }
    }
}
