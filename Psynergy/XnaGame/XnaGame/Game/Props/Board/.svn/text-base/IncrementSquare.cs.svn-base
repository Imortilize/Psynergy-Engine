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
    class IncrementSquare : ArithmeticSquare
    {
        #region Factory Property setting
        protected override void ClassProperties(Factory factory)
        {
            base.ClassProperties(factory);
        }
        #endregion

        public IncrementSquare() : base("")
        {
        }

        public IncrementSquare(String name) : base(name)
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
            // Decide what the 
            return new DecisionData(rollNumber + 1);
        }

        public override String GetSquareText()
        {
            return ("X++"); 
        }

        #region Properties
        #endregion
    }
}
