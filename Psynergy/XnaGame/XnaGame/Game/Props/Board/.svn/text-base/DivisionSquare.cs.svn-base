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
    public class DivisionSquare : ArithmeticSquare
    {
        #region Factory Property setting
        protected override void ClassProperties(Factory factory)
        {
            factory.RegisterInt("number", "Number");

            base.ClassProperties(factory);
        }
        #endregion

        private int m_Number = 0;

        public DivisionSquare() : base("")
        {
        }

        public DivisionSquare(String name) : base(name)
        {
        }

        public override void Initialise()
        {
            base.Initialise();
        }

        public override void Load()
        {
            base.Load();
        }

        public override void Update(GameTime deltaTime)
        {
            // Update base class where the main calculations occur.
            base.Update(deltaTime);
        }

        public override DecisionData Decision(int rollNumber)
        {
            DecisionData data = new DecisionData(rollNumber);

            if (m_Number <= 0)
                data.diceRoll = 0;
            else
                data.diceRoll = (rollNumber / m_Number);

            // Decide what the 
            return data;
        }

        public override String GetSquareText()
        {
            return ("X / " + m_Number);
        }

        #region Properties
        public int Number { get { return m_Number; } set { m_Number = value; } }
        #endregion
    }
}
