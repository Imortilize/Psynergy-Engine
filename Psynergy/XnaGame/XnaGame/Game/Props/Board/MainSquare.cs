using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XnaGame
{
    class MainSquare : BoardSquare
    {
        public MainSquare() : base("")
        {
        }

        public MainSquare(String name)
            : base(name)
        {
        }

        public override String GetSquareText()
        {
            return ("Main ()");
        }
    }
}
