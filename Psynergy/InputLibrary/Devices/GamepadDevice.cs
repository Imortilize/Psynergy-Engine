using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Psynergy.Input
{
    public class GamepadDevice : InputDevice
    {
        #region Enums
        #endregion

        #region Fields
        private PlayerIndex m_Index = PlayerIndex.One;
        private GamePadState m_OldState;
        private GamePadState m_CurrentState;
        #endregion

        #region Constructor
        public GamepadDevice(PlayerIndex playerIndex) : base()
        {
            m_Index = playerIndex;

            // Get start state
            m_OldState = GamePad.GetState(playerIndex);
            m_CurrentState = m_OldState;
        }
        #endregion

        #region Functions
        public override void Update(GameTime deltaTime)
        {
            // Save the current state as the old state
            if (m_CurrentState != null)
                m_OldState = m_CurrentState;

            // Get the current state
            m_CurrentState = GamePad.GetState(m_Index);
        }

        public override bool IsConnected()
        {
            bool toRet = false;

            if (m_CurrentState != null)
                toRet = m_CurrentState.IsConnected;

            return toRet;
        }

        public bool IsButtonPressed(ButtonState oldState, ButtonState newState)
        {
            return ((newState == ButtonState.Pressed) && (oldState == ButtonState.Released));
        }

        public bool IsButtonReleased(ButtonState oldState, ButtonState newState)
        {
            return ((newState == ButtonState.Released) && (oldState == ButtonState.Pressed));
        }

        public bool IsAnyInput()
        {
            bool toRet = false;

            if ((m_OldState != null) && (m_CurrentState != null))
                toRet = (m_OldState != m_CurrentState);

            return toRet;
        }

        #region Button Functions
        public bool GetStart()
        {
            bool toRet = false;

            if ((m_OldState != null) && (m_CurrentState != null))
            {
                if (m_OldState.IsConnected && m_CurrentState.IsConnected)
                    toRet = IsButtonPressed(m_OldState.Buttons.Start, m_CurrentState.Buttons.Start);
            }

            return toRet;
        }

        public bool GetButtonA()
        {
            bool toRet = false;

            if ((m_OldState != null) && (m_CurrentState != null))
            {
                if (m_OldState.IsConnected && m_CurrentState.IsConnected)
                    toRet = IsButtonPressed(m_OldState.Buttons.A, m_CurrentState.Buttons.A);
            }

            return toRet;
        }

        public bool GetButtonB()
        {
            bool toRet = false;

            if ((m_OldState != null) && (m_CurrentState != null))
            {
                if (m_OldState.IsConnected && m_CurrentState.IsConnected)
                    toRet = IsButtonPressed(m_OldState.Buttons.B, m_CurrentState.Buttons.B);
            }

            return toRet;
        }

        public bool GetButtonX()
        {
            bool toRet = false;

            if ((m_OldState != null) && (m_CurrentState != null))
            {
                if (m_OldState.IsConnected && m_CurrentState.IsConnected)
                    toRet = IsButtonPressed(m_OldState.Buttons.X, m_CurrentState.Buttons.X);
            }

            return toRet;
        }

        public bool GetButtonY()
        {
            bool toRet = false;

            if ((m_OldState != null) && (m_CurrentState != null))
            {
                if (m_OldState.IsConnected && m_CurrentState.IsConnected)
                    toRet = IsButtonPressed(m_OldState.Buttons.Y, m_CurrentState.Buttons.Y);
            }

            return toRet;
        }

        public bool GetBack()
        {
            bool toRet = false;

            if ((m_OldState != null) && (m_CurrentState != null))
            {
                if (m_OldState.IsConnected && m_CurrentState.IsConnected)
                    toRet = IsButtonPressed(m_OldState.Buttons.Back, m_CurrentState.Buttons.Back);
            }

            return toRet;
        }

        public bool GetDPadUp()
        {
            bool toRet = false;

            if ((m_OldState != null) && (m_CurrentState != null))
            {
                if (m_OldState.IsConnected && m_CurrentState.IsConnected)
                    toRet = IsButtonPressed(m_OldState.DPad.Up, m_CurrentState.DPad.Up);
            }

            return toRet;
        }

        public bool GetDPadDown()
        {
            bool toRet = false;

            if ((m_OldState != null) && (m_CurrentState != null))
            {
                if (m_OldState.IsConnected && m_CurrentState.IsConnected)
                    toRet = IsButtonPressed(m_OldState.DPad.Down, m_CurrentState.DPad.Down);
            }

            return toRet;
        }

        public bool GetDPadLeft()
        {
            bool toRet = false;

            if ((m_OldState != null) && (m_CurrentState != null))
            {
                if (m_OldState.IsConnected && m_CurrentState.IsConnected)
                    toRet = IsButtonPressed(m_OldState.DPad.Left, m_CurrentState.DPad.Left);
            }

            return toRet;
        }

        public bool GetDPadRight()
        {
            bool toRet = false;

            if ((m_OldState != null) && (m_CurrentState != null))
            {
                if (m_OldState.IsConnected && m_CurrentState.IsConnected)
                    toRet = IsButtonPressed(m_OldState.DPad.Right, m_CurrentState.DPad.Right);
            }

            return toRet;
        }

        public Vector2 GetLeftStick()
        {
            Vector2 toRet = Vector2.Zero;

            if (m_CurrentState != null)
            {
                if (m_CurrentState.IsConnected)
                    toRet = m_CurrentState.ThumbSticks.Left;
            }

            return toRet;
        }

        public Vector2 GetRightStick()
        {
            Vector2 toRet = Vector2.Zero;

            if (m_CurrentState != null)
            {
                if (m_CurrentState.IsConnected)
                    toRet = m_CurrentState.ThumbSticks.Right;
            }

            return toRet;
        }

        public bool GetLeftBumper()
        {
            bool toRet = false;

            if (m_CurrentState != null)
            {
                if (m_CurrentState.IsConnected)
                    toRet = m_CurrentState.IsButtonDown(Buttons.LeftShoulder);
            }

            return toRet;
        }

        public bool GetLeftBumperDown()
        {
            bool toRet = false;

            if ((m_OldState != null) && (m_CurrentState != null))
            {
                if (m_OldState.IsConnected && m_CurrentState.IsConnected)
                    toRet = IsButtonPressed(m_OldState.Buttons.LeftShoulder, m_CurrentState.Buttons.LeftShoulder);
            }

            return toRet;
        }

        public bool GetRightBumper()
        {
            bool toRet = false;

            if (m_CurrentState != null)
            {
                if (m_CurrentState.IsConnected)
                    toRet = m_CurrentState.IsButtonDown(Buttons.RightShoulder);
            }

            return toRet;
        }

        public bool GetRightBumperDown()
        {
            bool toRet = false;

            if ((m_OldState != null) && (m_CurrentState != null))
            {
                if (m_OldState.IsConnected && m_CurrentState.IsConnected)
                    toRet = IsButtonPressed(m_OldState.Buttons.RightShoulder, m_CurrentState.Buttons.RightShoulder);
            }

            return toRet;
        }

        public float GetLeftTrigger()
        {
            float toRet = 0.0f;

            if (m_CurrentState != null)
            {
                if (m_CurrentState.IsConnected)
                    toRet = m_CurrentState.Triggers.Left;
            }

            return toRet;
        }

        public float GetRightTrigger()
        {
            float toRet = 0.0f;

            if (m_CurrentState != null)
            {
                if (m_CurrentState.IsConnected)
                    toRet = m_CurrentState.Triggers.Right ;
            }

            return toRet;
        }
        #endregion
        #endregion

        #region Properties
        #endregion
    }
}
