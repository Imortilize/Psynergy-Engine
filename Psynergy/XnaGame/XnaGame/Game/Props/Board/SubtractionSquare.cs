using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

/* Main Library */
using Psynergy;

namespace XnaGame
{
    class SubtractionSquare : ArithmeticSquare
    {
        #region Factory Property setting
        protected override void ClassProperties(Factory factory)
        {
            factory.RegisterInt("number", "Number");
            factory.RegisterInt("subtractrollnumber", "SubtractRollNumber");

            base.ClassProperties(factory);
        }
        #endregion

        private int m_SubtractRollNumber = 0;
        private int m_Number = 0;

        public SubtractionSquare() : base("")
        {
        }

        public SubtractionSquare(String name) : base(name)
        {
        }

        public override DecisionData Decision(int rollNumber)
        {
            DecisionData data = new DecisionData();

            if (m_SubtractRollNumber == 0)
                data.diceRoll = (m_Number - rollNumber);
            else
                data.diceRoll = (rollNumber - m_Number);

            // Decide what the 
            return data;
        }

        public override String GetSquareText()
        {
            String toRet = "";

            if (m_SubtractRollNumber == 0)
                toRet = (m_Number + " - X");
            else
                toRet = ("X - " + m_Number);

            return toRet;
        }

        #region Properties
        public int Number 
        { 
            get 
            { 
                return m_Number; 
            } 
            set 
            { 
                m_Number = value;

                if (value < 0)
                    m_Number = 0;
            } 
        }

        public int SubtractRollNumber
        {
            get
            {
                return m_SubtractRollNumber;
            }
            set
            {
                m_SubtractRollNumber = value;

                if (value < 0)
                    m_SubtractRollNumber = 0;
            }
        }
        #endregion
    }
}
