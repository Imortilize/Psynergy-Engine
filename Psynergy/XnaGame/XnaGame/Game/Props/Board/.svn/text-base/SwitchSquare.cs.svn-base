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
    class SwitchSquare : BoardSquare
    {
        #region Factory Property setting
        protected override void ClassProperties(Factory factory)
        {
            base.ClassProperties(factory);
        }
        #endregion

        public SwitchSquare() : base("")
        {
        }

        public SwitchSquare(String name) : base(name)
        {
        }

        public override DecisionData Decision(int rollNumber)
        {
            DecisionData data = new DecisionData(rollNumber);

            if (rollNumber == 1)
                data.route = Route.eAlternative1;
            else if (rollNumber == 2)
                data.route = Route.eAlternative2;
            else if (rollNumber == 3)
                data.route = Route.eAlternative3; 

            // Decide what the 
            return data;
        }

        public override String GetSquareText()
        {
            return ("Switch (X)");
        }

        #region Properties
        #endregion
    }
}
