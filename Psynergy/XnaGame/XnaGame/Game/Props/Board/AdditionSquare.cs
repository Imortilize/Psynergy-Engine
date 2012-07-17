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
    class AdditionSquare : ArithmeticSquare
    {
        #region Factory Property setting
        protected override void ClassProperties(Factory factory)
        {
            factory.RegisterInt("number", "Number");

            base.ClassProperties(factory);
        }
        #endregion

        private int m_Number = 0;

        public AdditionSquare() : base("")
        {
        }

        public AdditionSquare(String name) : base(name)
        {
        }

        public override DecisionData Decision(int rollNumber)
        {
            // Decide what the 
            return new DecisionData(rollNumber + m_Number);
        }

        public override String GetSquareText()
        {
            return ("X + " + m_Number);
        }

        #region Properties
        public int Number { get { return m_Number; } set { m_Number = value; } }
        #endregion
    }
}
