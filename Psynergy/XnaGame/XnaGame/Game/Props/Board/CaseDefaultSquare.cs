using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XnaGame
{
    class CaseDefaultSquare : BoardSquare
    {
        public CaseDefaultSquare() : base("")
        {
        }

        public CaseDefaultSquare(String name)
            : base(name)
        {
        }

        public override String GetSquareText()
        {
            return ("default");
        }
    }
}
