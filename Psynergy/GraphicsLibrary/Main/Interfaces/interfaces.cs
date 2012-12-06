using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Instrumentation;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Psynergy.Graphics
{
    public interface IDeferrable : IInterface
    {
        #region Type comparers
        bool IsTypeOf<T>() where T : GameObject;

        bool InheritsFrom<T>() where T : GameObject;
        #endregion
    }
}
