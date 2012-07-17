using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XnaGame
{
    class IntXSquare : BoardSquare
    {
        public IntXSquare() : base("")
        {
        }

        public IntXSquare(String name)
            : base(name)
        {
        }

        public override String GetSquareText()
        {
            return ("Int X");
        }
    }
}
