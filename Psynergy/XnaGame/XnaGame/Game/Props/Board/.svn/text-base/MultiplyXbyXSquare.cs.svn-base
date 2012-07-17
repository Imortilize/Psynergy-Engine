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
    class MultiplyXByXSquare : ArithmeticSquare
    {
        #region Factory Property setting
        protected override void ClassProperties(Factory factory)
        {
            base.ClassProperties(factory);
        }
        #endregion

        public MultiplyXByXSquare() : base("")
        {
        }

        public MultiplyXByXSquare(String name) : base(name)
        {
        }

        public override void Update(GameTime deltaTime)
        {
            // Update base class where the main calculations occur.
            base.Update(deltaTime);
        }

        public override DecisionData Decision(int rollNumber)
        {
            DecisionData data = new DecisionData(rollNumber);
            data.diceRoll = (rollNumber * rollNumber);

            // Decide what the 
            return data;
        }

        public override String GetSquareText()
        {
            return ("X * X");
        }

        #region Properties
        #endregion
    }
}
