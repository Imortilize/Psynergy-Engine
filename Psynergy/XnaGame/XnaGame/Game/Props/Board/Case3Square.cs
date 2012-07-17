using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XnaGame
{
    class Case3Square : BoardSquare
    {
        public Case3Square() : base("")
        {
        }

        public Case3Square(String name)
            : base(name)
        {
        }

        public override String GetSquareText()
        {
            return ("case 3");
        }
    }
}
