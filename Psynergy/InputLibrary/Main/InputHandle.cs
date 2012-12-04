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
    public static class InputHandle
    {
        #region Enums
        #endregion

        #region Fields
        // Graphics handle
        private static GraphicsDevice m_GraphicsDevice = null;

        // Mouse Handle
        private static MouseDevice m_MouseDevice = null;

        // Keyboard handle
        private static KeyboardDevice m_KeyboardDevice = null;

        // Gamepad handles
        private static SortedList<PlayerIndex, GamepadDevice> m_GamepadDevices = new SortedList<PlayerIndex, GamepadDevice>();

        // Extra values
        private static bool m_CanPause = false;
        private static bool m_PauseApplication = false;
        private static bool m_PauseRenderering = false;
        private static bool m_ExitApplication = false;
        #endregion

        #region Functions
        #region General Functions
        public static void Update(GameTime deltaTime)
        {
            // Update pause
            UpdatePause(deltaTime);

            // Update mouse
            UpdateMouse(deltaTime);

            // Update Keyboard
            UpdateKeyboard(deltaTime);

            // Update Gamepads
            UpdateGamePads(deltaTime);
        }
        #endregion

        #region Generic Functions
        public static void Exit()
        {
            m_ExitApplication = true;
        }

        #region Pause Functions
        private static void UpdatePause(GameTime deltaTime)
        {
            // If the user is allowed to pause or not
            if (m_CanPause)
            {
                // Check if the application is asking to be paused or not
                if (CheckPause())
                {
                    // inverse the pause application booleans ( if true it gets turned to false, otherwise turn it to true ).
                    SwitchPause();
                }
            }
        }

        public static void ActivatePause()
        {
            m_CanPause = true;
        }

        public static void DeactivatePause()
        {
            m_CanPause = false;
        }

        public static void SwitchPause()
        {
            if (!PauseApplication)
                PauseGameMenu();
            else
                UnPauseGame();
        }

        /* This is used if wanting to pause the game outside of the default pause game buttons ( such as a cutscene pause ) etc.. */
        public static void PauseGame(bool show)
        {
            m_PauseApplication = show;

            // State that the user cannot use the in game pause buttons whilst this is active
            m_CanPause = false;
        }

        /* This is an auto pause menu if pause is enabled and the pause button is pressed in game. It shows the pause menu as well */
        private static void PauseGameMenu()
        {
            m_PauseApplication = true;

            // Show the pause menu
            PauseGameEvent pauseEvent = new PauseGameEvent(true);
            pauseEvent.Fire();
        }

        public static void UnPauseGame()
        {
            m_PauseApplication = false;

            // Close the pause menu ( shouldn't make a different whether it was opened to start with or nots
            PauseGameEvent pauseEvent = new PauseGameEvent(false);
            pauseEvent.Fire();

            // State that the user can pause the game again 
            // ( this should only really be automatic if the game was paused outside of the pause menu buttons)
            m_CanPause = true;
        }

        public static void PauseRendering(bool pause)
        {
            m_PauseRenderering = pause;
        }
        #endregion
        #endregion

        #region Mouse Functions
        private static void UpdateMouse(GameTime deltaTime)
        {
            MouseDevice device = GetMouseDevice();
            if (device != null)
                device.Update(deltaTime);
        }

        public static bool GetMouse(int index)
        {
            bool toRet = false;

            MouseDevice device = GetMouseDevice();
            if (device != null)
                toRet = device.GetMouse(index);

            return toRet;
        }

        public static bool GetMouseDown(int index)
        {
            bool toRet = false;

            MouseDevice device = GetMouseDevice();
            if (device != null)
                toRet = device.GetMouseDown(index);

            return toRet;
        }

        public static bool GetMouseUp(int index)
        {
            bool toRet = false;

            MouseDevice device = GetMouseDevice();
            if (device != null)
                toRet = device.GetMouseUp(index);

            return toRet;
        }

        public static bool IsMouseConnected()
        {
            bool toRet = false;

            MouseDevice device = GetMouseDevice();
            if (device != null)
                toRet = device.IsConnected();

            return toRet;
        }

        private static MouseDevice GetMouseDevice()
        {
            if (m_MouseDevice == null)
                m_MouseDevice = new MouseDevice();

            if (m_MouseDevice != null)
            {
                if (m_GraphicsDevice != null)
                    m_MouseDevice.ViewPort = m_GraphicsDevice.Viewport;
            }

            return m_MouseDevice;
        }
        #endregion

        #region Keyboard Functions
        public static bool GetKey(Keys key)
        {
            bool toRet = false;

            KeyboardDevice device = GetKeyboardDevice();
            if (device != null)
                toRet = device.GetKey(key);

            return toRet;
        }

        public static bool GetKeyDown(Keys key)
        {
            bool toRet = false;

            KeyboardDevice device = GetKeyboardDevice();
            if (device != null)
                toRet = device.GetKeyDown(key);

            return toRet;
        }

        public static bool GetKeyUp(Keys key)
        {
            bool toRet = false;

            KeyboardDevice device = GetKeyboardDevice();
            if (device != null)
                toRet = device.GetKeyUp(key);

            return toRet;
        }

        private static void UpdateKeyboard(GameTime deltaTime)
        {
            KeyboardDevice device = GetKeyboardDevice();
            if (device != null)
                device.Update(deltaTime);
        }

        private static KeyboardDevice GetKeyboardDevice()
        {
            if (m_KeyboardDevice == null)
                m_KeyboardDevice = new KeyboardDevice();

            return m_KeyboardDevice;
        }
        #endregion

        #region Gamepad Functions
        public static Vector2 LeftStick(PlayerIndex playerIndex)
        {
            Vector2 toRet = new Vector2(0, 0);

            GamepadDevice gamepad = GetGamepadDevice(playerIndex);
            if (gamepad != null)
                toRet = gamepad.GetLeftStick();

            return toRet;
        }

        public static Vector2 RightStick(PlayerIndex playerIndex)
        {
            Vector2 toRet = new Vector2(0, 0);

            GamepadDevice gamepad = GetGamepadDevice(playerIndex);
            if (gamepad != null)
                toRet = gamepad.GetRightStick();

            return toRet;
        }

        public static bool LeftBumperDown(PlayerIndex playerIndex)
        {
            bool toRet = false;

            GamepadDevice gamepad = GetGamepadDevice(playerIndex);
            if (gamepad != null)
                toRet = gamepad.GetLeftBumper();

            return toRet;
        }

        public static bool RightBumperDown(PlayerIndex playerIndex)
        {
            bool toRet = false;

            GamepadDevice gamepad = GetGamepadDevice(playerIndex);
            if (gamepad != null)
                toRet = gamepad.GetRightBumper();

            return toRet;
        }

        public static bool LeftBumperPressed(PlayerIndex playerIndex)
        {
            bool toRet = false;

            GamepadDevice gamepad = GetGamepadDevice(playerIndex);
            if (gamepad != null)
                toRet = gamepad.GetLeftBumperDown();

            return toRet;
        }

        public static bool RightBumperPressed(PlayerIndex playerIndex)
        {
            bool toRet = false;

            GamepadDevice gamepad = GetGamepadDevice(playerIndex);
            if (gamepad != null)
                toRet = gamepad.GetRightBumperDown();

            return toRet;
        }

        public static float LeftTrigger(PlayerIndex playerIndex)
        {
            float toRet = 0.0f;

            GamepadDevice gamepad = GetGamepadDevice(playerIndex);
            if (gamepad != null)
                toRet = gamepad.GetLeftTrigger();

            return toRet;
        }

        public static float RightTrigger(PlayerIndex playerIndex)
        {
            float toRet = 0.0f;

            GamepadDevice gamepad = GetGamepadDevice(playerIndex);
            if (gamepad != null)
                toRet = gamepad.GetRightTrigger();

            return toRet;
        }

        public static bool IsGamePadConnected(PlayerIndex playerIndex)
        {
            bool toRet = false;

            GamepadDevice gamepad = GetGamepadDevice(playerIndex);
            if (gamepad != null)
                toRet = gamepad.IsConnected();

            return toRet;
        }

        private static void UpdateGamePads(GameTime deltaTime)
        {
            // Check to see if the old state shoudl be saved or not
            for (int i = 0; i < m_GamepadDevices.Values.Count; i++)
            {
                GamepadDevice gamePad = m_GamepadDevices.Values[i];
                if (gamePad != null)
                    gamePad.Update(deltaTime);
            }
        }

        private static GamepadDevice GetGamepadDevice(PlayerIndex playerIndex)
        {
            GamepadDevice device = null;

            int index = m_GamepadDevices.IndexOfKey(playerIndex);
            if (index < 0)
            {
                device = new GamepadDevice(playerIndex);

                // Add to the game pad devices list
                m_GamepadDevices.Add(playerIndex, device);
            }
            else
                device = m_GamepadDevices.Values[index];

            return device;
        }
        #endregion

        #region Game Related Common Functions
        public static bool IsAnyInput(PlayerIndex playerIndex)
        {
            if (GetMouseDown(0) || GetMouseDown(1) || GetMouseDown(2))
                return true;

            KeyboardDevice device = GetKeyboardDevice();
            if (device != null)
            {
                if (device.GetKeysPressed().Length > 0)
                    return true;
            }

            GamepadDevice gamepad = GetGamepadDevice(playerIndex);
            if (gamepad != null)
            {
                if (gamepad.IsConnected())
                {
                    if (gamepad.IsAnyInput())
                        return true;
                }
            }

            return false;
        }

        public static bool CheckPause()
        {
            bool toRet = false;

            if (InputHandle.GetKeyDown(Keys.P) || InputHandle.GetKeyDown(Keys.Escape))
                toRet = true;
            else
            {
                GamepadDevice device = GetGamepadDevice(PlayerIndex.One);
                if (device != null)
                    toRet = device.GetStart();
            }

            return toRet;
        }

        // If Ok is selected from any device
        public static bool Submit(PlayerIndex playerIndex)
        {
            bool toRet = false;

            if (InputHandle.GetKeyDown(Keys.Enter) || InputHandle.GetMouseDown(0))
                toRet = true;
            else
            {
                GamepadDevice device = GetGamepadDevice(PlayerIndex.One);
                if (device != null)
                    toRet = device.GetButtonA();
            }

            return toRet;
        }

        // If back is selected from any device
        public static bool Return(PlayerIndex playerIndex)
        {
            bool toRet = false;

            if (InputHandle.GetKeyDown(Keys.Back))
                toRet = true;
            else
            {
                GamepadDevice device = GetGamepadDevice(PlayerIndex.One);
                if (device != null)
                    toRet = device.GetButtonB();
            }

            return toRet;
        }

        // If back is selected from any device
        public static bool Back(PlayerIndex playerIndex)
        {
            bool toRet = false;

            if (InputHandle.GetKeyDown(Keys.Back))
                toRet = true;
            else
            {
                GamepadDevice device = GetGamepadDevice(PlayerIndex.One);
                if (device != null)
                    toRet = device.GetBack();
            }

            return toRet;
        }

        // If UP is selected from any device
        public static bool Up(PlayerIndex playerIndex)
        {
            bool toRet = false;

            if (InputHandle.GetKeyDown(Keys.Up))
                toRet = true;
            else
            {
                GamepadDevice device = GetGamepadDevice(PlayerIndex.One);
                if (device != null)
                    toRet = device.GetDPadUp();
            }

            return toRet;
        }

        // If DOWN is selected from any device
        public static bool Down(PlayerIndex playerIndex)
        {
            bool toRet = false;

            if (InputHandle.GetKeyDown(Keys.Down))
                toRet = true;
            else
            {
                GamepadDevice device = GetGamepadDevice(PlayerIndex.One);
                if (device != null)
                    toRet = device.GetDPadDown();
            }

            return toRet;
        }

        // If LEFT is selected from any device
        public static bool Left(PlayerIndex playerIndex)
        {
            bool toRet = false;

            if (InputHandle.GetKeyDown(Keys.Left))
                toRet = true;
            else
            {
                GamepadDevice device = GetGamepadDevice(PlayerIndex.One);
                if (device != null)
                    toRet = device.GetDPadLeft();
            }

            return toRet;
        }

        // If RIGHT is selected from any device
        public static bool Right(PlayerIndex playerIndex)
        {
            bool toRet = false;

            if (InputHandle.GetKeyDown(Keys.Right))
                toRet = true;
            else
            {
                GamepadDevice device = GetGamepadDevice(PlayerIndex.One);
                if (device != null)
                    toRet = device.GetDPadRight();
            }

            return toRet;
        }
        #endregion
        #endregion

        #region Properties
        public static GraphicsDevice GraphicsDevice { set { m_GraphicsDevice = value; } }
        public static bool CanPause { get { return m_CanPause; } }
        public static bool PauseApplication { get { return m_PauseApplication; } set { m_PauseApplication = false; } }
        public static bool PauseDraw { get { return m_PauseRenderering; } }
        public static bool ExitApplication { get { return m_ExitApplication; } }

        #region Mouse Properties
        public static Vector2 MousePosition 
        { 
            get 
            {
                Vector2 position = Vector2.Zero;

                // look for the device
                MouseDevice device = GetMouseDevice();
                if (device != null)
                    position = device.MousePosition;

                return position;
            } 
        }

        public static Vector3 MouseDelta
        {
            get
            {
                Vector3 delta = Vector3.Zero;

                // look for the device
                MouseDevice device = GetMouseDevice();
                if (device != null)
                    delta = device.MouseDelta;

                return delta;
            }
        }
        #endregion
        #endregion
    }
}
