using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace Psynergy.Input
{
    public class InputDevice
    {
        #region Fields
        #endregion

        #region Constructor
        public InputDevice()
        {
        }
        #endregion

        #region Functions
        public virtual void Update(GameTime deltaTime)
        {
        }

        public virtual bool IsConnected()
        {
            return false;
        }
        #endregion

        #region Properties
        #endregion
    }
}
