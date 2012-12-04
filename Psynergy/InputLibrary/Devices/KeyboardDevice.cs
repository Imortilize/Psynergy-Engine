using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Psynergy.Input   
{
    public class KeyboardDevice : InputDevice
    {
        #region Fields
        // Keyboard values
        private KeyboardState m_OldKeyboardState;
        private KeyboardState m_CurrentKeyboardState;
        #endregion

        #region Constructor
        public KeyboardDevice() : base()
        {
            // Get the start keyboard state
            m_OldKeyboardState = Keyboard.GetState();
            m_CurrentKeyboardState = Keyboard.GetState();
        }
        #endregion

        #region Functions
        public override void Update(GameTime deltaTime)
        {
            base.Update(deltaTime);

            // Set the old keyboard state
            m_OldKeyboardState = m_CurrentKeyboardState;

            // Set the current keyboard state
            m_CurrentKeyboardState = Keyboard.GetState();
        }

        public bool GetKey(Keys key)
        {
            bool toRet = false;

            // Get the new keyboard state
            if (m_CurrentKeyboardState.IsKeyDown(key))
                toRet = true;

            return toRet;
        }

        public bool GetKeyDown(Keys key)
        {
            bool toRet = false;

            // Check if the key was pressed last frame or not
            if (m_OldKeyboardState.IsKeyUp(key))
            {
                // If not, then it could have been pressed this cycle so see if the current state says it's pressed or not
                if (m_CurrentKeyboardState.IsKeyDown(key))
                {
                    // Key was pressed this cycle.
                    toRet = true;
                }
            }

            return toRet;
        }

        public bool GetKeyUp(Keys key)
        {
            bool toRet = false;

            // Check if the key was pressed last frame or not
            if (m_OldKeyboardState.IsKeyDown(key))
            {
                // If it was, then it could have been released this cycle so see if the current state says it's released or not
                if (m_CurrentKeyboardState.IsKeyUp(key))
                {
                    // Key was pressed this cycle.
                    toRet = true;
                }
            }

            return toRet;
        }

        public Keys[] GetKeysPressed()
        {
            KeyboardState newState = Keyboard.GetState();

            // This returns all the keys that have been pressed this cycle
            return newState.GetPressedKeys();
        }

        public override bool IsConnected()
        {
            // Apparently best to treat this as if it is always connection is the way to do it...
            return true;
        }
        #endregion

        #region Properties
        #endregion
    }
}
