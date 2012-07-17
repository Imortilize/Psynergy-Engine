using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XnaGame
{
    class OpenBracketSquare : BoardSquare
    {
        public OpenBracketSquare() : base("")
        {
        }

        public OpenBracketSquare(String name)
            : base(name)
        {
        }

        public override String GetSquareText()
        {
            return ("{");
        }
    }
}
