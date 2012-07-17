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
    public class DivideXByXSquare : ArithmeticSquare
    {
        #region Factory Property setting
        protected override void ClassProperties(Factory factory)
        {
            base.ClassProperties(factory);
        }
        #endregion

        public DivideXByXSquare() : base("")
        {
        }

        public DivideXByXSquare(String name) : base(name)
        {
        }

        public override DecisionData Decision(int rollNumber)
        {
            DecisionData data = new DecisionData(rollNumber);

            if (rollNumber <= 0)
                data.diceRoll = 0;
            else
                data.diceRoll = (rollNumber / rollNumber);

            // Decide what the 
            return data;
        }

        public override String GetSquareText()
        {
            return ("X / X");
        }

        #region Properties
        #endregion
    }
}
