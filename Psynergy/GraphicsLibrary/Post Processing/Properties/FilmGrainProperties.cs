using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Psynergy.Graphics
{
    public class FilmGrainProperties : AbstractPostProcessorProperties
    {
        private float m_FilmGrainStrength = 0;
        private float m_DarkNoisePower = 1;
        private float m_RandomNoiseStrength = 0;

        public FilmGrainProperties()
            : base()
        {
            FilmGrainStrength = 0;
            DarkNoisePower = 1;
            RandomNoiseStrength = 0;
        }

        public FilmGrainProperties(float grainStrength)
            : base(true)
        {
            FilmGrainStrength = grainStrength;
            DarkNoisePower = 1;
            RandomNoiseStrength = 0;
        }

        public FilmGrainProperties(float grainStrength, float darkNoisePower, float randomNoiseStrength)
            : base(true)
        {
            FilmGrainStrength = grainStrength;
            DarkNoisePower = darkNoisePower;
            RandomNoiseStrength = randomNoiseStrength;
        }

        public float FilmGrainStrength
        {
            get { return m_FilmGrainStrength; }
            set
            {
                if (value < 0)
                    value = 0;
                else if (value > 1)
                    value = 1;

                m_FilmGrainStrength = value;
            }
        }

        public float DarkNoisePower
        {
            get { return m_DarkNoisePower; }
            set
            {
                if (value < 1)
                    value = 1;
                else if (value > 5)
                    value = 5;

                m_DarkNoisePower = value;
            }
        }

        public float RandomNoiseStrength
        {
            get { return m_RandomNoiseStrength; }
            set
            {
                if (value < 0)
                    value = 0;

                m_RandomNoiseStrength = value;
            }
        }
    };

}
