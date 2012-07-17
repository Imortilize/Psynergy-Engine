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
    class WhileSquare : BoardSquare
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

        public WhileSquare() : base("")
        {
        }

        public WhileSquare(String name) : base(name)
        {
        }

        public override DecisionData Decision(int rollNumber)
        {
            DecisionData data = new DecisionData(rollNumber);

            if (m_DecisionFlag == "equal")
            {
                if (rollNumber == m_Number)
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
                toRet = ("while (X == " + m_Number + ")");
            else if (m_DecisionFlag == "lessthan")
                toRet = ("while (X < " + m_Number + ")");
            else if (m_DecisionFlag == "greaterthan")
                toRet = ("while (X > " + m_Number + ")");

            return toRet;
        }

        #region Properties
        public int Number { get { return m_Number; } set { m_Number = value; } }
        public String Flag { get { return m_DecisionFlag; } set { m_DecisionFlag = value; } }
        #endregion
    }
}
