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
    class IfSquare : BoardSquare
    {
        #region Factory Property setting
        protected override void ClassProperties(Factory factory)
        {
            factory.RegisterInt("number", "Number");
            factory.RegisterString("flag", "Flag");

            base.ClassProperties(factory);
        }
        #endregion

        private int m_Number = 0;
        private String m_DecisionFlag = "equal";

        public IfSquare() : base("")
        {
        }

        public IfSquare(String name) : base(name)
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

            if (m_DecisionFlag == "equal")
            {
                if (rollNumber == m_Number )
                    data.route = Route.eAlternative1;
            }
            else if (m_DecisionFlag == "lessthan")
            {
                if (rollNumber < m_Number)
                    data.route = Route.eAlternative1;
            }
            else if (m_DecisionFlag == "greaterthan")
            {
                if (rollNumber > m_Number)
                    data.route = Route.eAlternative1;
            }
             
            // Decide what the 
            return data;
        }

        public override String GetSquareText()
        {
            String toRet = "";

            if (m_DecisionFlag == "equal")
                toRet = ("if (X == " + m_Number + ")");
            else if (m_DecisionFlag == "lessthan")
                toRet = ("if (X < " + m_Number + ")");
            else if (m_DecisionFlag == "greaterthan")
                toRet = ("if (X > " + m_Number + ")");

            return toRet; ;
        }

        #region Properties
        public int Number { get { return m_Number; } set { m_Number = value; } }
        public String Flag { get { return m_DecisionFlag; } set { m_DecisionFlag = value; } }
        #endregion
    }
}
