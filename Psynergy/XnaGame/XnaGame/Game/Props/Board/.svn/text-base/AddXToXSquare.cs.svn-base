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
    class AddXToXSquare : ArithmeticSquare
    {
        #region Factory Property setting
        protected override void ClassProperties(Factory factory)
        {
            base.ClassProperties(factory);
        }
        #endregion

        public AddXToXSquare() : base("")
        {
        }

        public AddXToXSquare(String name) : base(name)
        {
        }

        public override DecisionData Decision(int rollNumber)
        {
            // Decide what the 
            return new DecisionData(rollNumber + rollNumber);
        }

        public override String GetSquareText()
        {
            return ("X + X");
        }

        #region Properties
        #endregion
    }
}
