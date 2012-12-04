using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

/*namespace Psynergy.Input
{
    public class InputManager : Singleton<InputManager>
    {
        // Viewport
        //private Viewport m_ViewPort;

        // Gamepad values
        /*struct GamePadTracker
        {
            public GamePadTracker(PlayerIndex index, GamePadState startState)
            {
                playerIndex = index;
                oldState = startState;
                currentState = startState;
            }

            public PlayerIndex PlayerIndex { get { return playerIndex; } set { playerIndex = value; } }
            public GamePadState OldState { get { return oldState; } set { oldState = value; } }
            public GamePadState CurrentState { get { return currentState; } set { currentState = value; } }

            public PlayerIndex playerIndex;
            public GamePadState oldState;
            public GamePadState currentState;
        };

        private SortedList<PlayerIndex, GamePadTracker> m_GamePadTrackers = new SortedList<PlayerIndex, GamePadTracker>();*/

        /*private bool m_CanPause = false;
        private bool m_PauseApplication = false;
        private bool m_PauseRenderering = false;
        private bool m_ExitApplication = false;*/

       /* public InputManager()
        {
            //m_ViewPort = new Viewport();
        }

        public override void Initialise()
        {
            // get the game pad states
           // InitialiseGamePads();

            base.Initialise();
        }*/

       /* private void InitialiseGamePads()
        {
            m_GamePadTrackers.Add(PlayerIndex.One, new GamePadTracker(PlayerIndex.One, GamePad.GetState(PlayerIndex.One)));
            m_GamePadTrackers.Add(PlayerIndex.Two, new GamePadTracker(PlayerIndex.Two, GamePad.GetState(PlayerIndex.Two)));
            m_GamePadTrackers.Add(PlayerIndex.Three, new GamePadTracker(PlayerIndex.Three, GamePad.GetState(PlayerIndex.Three)));
            m_GamePadTrackers.Add(PlayerIndex.Four, new GamePadTracker(PlayerIndex.Four, GamePad.GetState(PlayerIndex.Four)));
        }*/

       /* public override void Load()
        {
            base.Load();
        }

        public override void Update(GameTime deltaTime)
        {
            base.Update(deltaTime);*/
            
            // If the user is allowed to pause or not
            /*if (m_CanPause)
            {
                // Check if the application is asking to be paused or not
                if (CheckPause())
                {
                    // inverse the pause application booleans ( if true it gets turned to false, otherwise turn it to true ).
                    SwitchPause();
                }
            }*/

            // Update gamepad values
           // UpdateGamePads(deltaTime);
       //67 }

       /* private void UpdateGamePads(GameTime deltaTime)
        {
            // Check to see if the old state shoudl be saved or not
            for (int i = 0; i < m_GamePadTrackers.Values.Count; i++)
            {
                GamePadTracker gamePad = m_GamePadTrackers.Values[i];

                // Save the current state as the old state
                if (gamePad.CurrentState != null)
                    gamePad.OldState = gamePad.CurrentState;

                // Get the current state
                gamePad.CurrentState = GamePad.GetState(gamePad.PlayerIndex);

                // If the game pad is connected then we will remove and readd the controller
                if (gamePad.CurrentState.IsConnected)
                {
                    // Remove the gamepad first
                    m_GamePadTrackers.RemoveAt(i);

                    // Re-add the controller
                    m_GamePadTrackers.Add(gamePad.PlayerIndex, gamePad);
                }
            }
        }*/

        // GamePad functions
        /*public bool IsButtonPressed(ButtonState oldState, ButtonState newState)
        {
            return ((newState == ButtonState.Pressed) && (oldState == ButtonState.Released));
        }

        public bool IsButtonReleased(ButtonState oldState, ButtonState newState)
        {
            return ((newState == ButtonState.Released) && (oldState == ButtonState.Pressed));
        }*/

        /* Pause functions */
        /*public void ActivatePause()
        {
            m_CanPause = true;
        }

        public void DeactivatePause()
        {
            m_CanPause = false;
        }*/

        /*private bool CheckPause()
        {
            bool toRet = false;

            if (InputHandle.GetKeyDown(Keys.P) || InputHandle.GetKeyDown(Keys.Escape))
                toRet = true;
            else
            {
                int index = m_GamePadTrackers.IndexOfKey(PlayerIndex.One);

                Debug.Assert(index >= 0, "Player index " + index + " was not found in the game pad trackers!");

                if (index >= 0)
                {
                    GamePadState oldState = m_GamePadTrackers.Values[index].oldState;
                    GamePadState currentState = m_GamePadTrackers.Values[index].currentState;

                    if (oldState.IsConnected && currentState.IsConnected)
                        toRet = IsButtonPressed(oldState.Buttons.Start, currentState.Buttons.Start);
                }
            }

            return toRet;
        }*/

       /* public void SwitchPause()
        {
            if (!PauseApplication)
                PauseGameMenu();
            else
                UnPauseGame();
        }

        /* This is used if wanting to pause the game outside of the default pause game buttons ( such as a cutscene pause ) etc.. */
        /*public void PauseGame(bool show)
        {
            PauseApplication = show;

            // State that the user cannot use the in game pause buttons whilst this is active
            m_CanPause = false;
        }

        /* This is an auto pause menu if pause is enabled and the pause button is pressed in game. It shows the pause menu as well */
       /* private void PauseGameMenu()
        {
            PauseApplication = true;

            // Show the pause menu
            PauseGameEvent pauseEvent = new PauseGameEvent(true);
            pauseEvent.Fire();
        }

        public void UnPauseGame()
        {
            PauseApplication = false;

            // Close the pause menu ( shouldn't make a different whether it was opened to start with or nots
            PauseGameEvent pauseEvent = new PauseGameEvent(false);
            pauseEvent.Fire();

            // State that the user can pause the game again 
            // ( this should only really be automatic if the game was paused outside of the pause menu buttons)
            m_CanPause = true;
        }

        public void PauseRendering(bool pause)
        {
            m_PauseRenderering = pause;
        }*/

        // If Ok is selected from any device
        /*public bool Submit(PlayerIndex playerIndex)
        {
            bool toRet = false;

            if (InputHandle.GetKeyDown(Keys.Enter) || InputHandle.GetMouseDown(0))
                toRet = true;
            else
            {
                int index = m_GamePadTrackers.IndexOfKey(playerIndex);

                Debug.Assert(index >= 0, "Player index " + playerIndex + " was not found in the game pad trackers!");

                if (index >= 0)
                {
                    GamePadState oldState = m_GamePadTrackers.Values[index].oldState;
                    GamePadState currentState = m_GamePadTrackers.Values[index].currentState;

                    if (oldState.IsConnected && currentState.IsConnected)
                        toRet = IsButtonPressed(oldState.Buttons.A, currentState.Buttons.A);
                }
            }

            return toRet;
        }

        // If back is selected from any device
        public bool Return(PlayerIndex playerIndex)
        {
            bool toRet = false;

            if (InputHandle.GetKeyDown(Keys.Back))
                toRet = true;
            else
            {
                int index = m_GamePadTrackers.IndexOfKey(playerIndex);

                GamePadState oldState = m_GamePadTrackers.Values[index].oldState;
                GamePadState currentState = m_GamePadTrackers.Values[index].currentState;

                if (oldState.IsConnected && currentState.IsConnected)
                    toRet = IsButtonPressed(oldState.Buttons.B, currentState.Buttons.B);
            }

            return toRet;
        }

        public bool Click(PlayerIndex playerIndex)
        {
            bool toRet = false;

            if (InputHandle.GetMouseDown(0))
                toRet = true;
            
            return toRet;
        }*/

        // If back is selected from any device
        /*public bool Back(PlayerIndex playerIndex)
        {
            bool toRet = false;

            if (InputHandle.GetKeyDown(Keys.Back))
                toRet = true;
            else
            {
                int index = m_GamePadTrackers.IndexOfKey(playerIndex);

                Debug.Assert(index >= 0, "Player index " + playerIndex + " was not found in the game pad trackers!");

                if (index >= 0)
                {
                    GamePadState oldState = m_GamePadTrackers.Values[index].oldState;
                    GamePadState currentState = m_GamePadTrackers.Values[index].currentState;

                    if (oldState.IsConnected && currentState.IsConnected)
                        toRet = IsButtonPressed(oldState.Buttons.Back, currentState.Buttons.Back);
                }
            }

            return toRet;
        }

        // If UP is selected from any device
        public bool Up(PlayerIndex playerIndex)
        {
            bool toRet = false;

            if (InputHandle.GetKeyDown(Keys.Up))
                toRet = true;
            else
            {
                int index = m_GamePadTrackers.IndexOfKey(playerIndex);

                GamePadState oldState = m_GamePadTrackers.Values[index].oldState;
                GamePadState currentState = m_GamePadTrackers.Values[index].currentState;

                if (oldState.IsConnected && currentState.IsConnected)
                    toRet = IsButtonPressed(oldState.DPad.Up, currentState.DPad.Up);
            }

            return toRet;
        }

        // If DOWN is selected from any device
        public bool Down(PlayerIndex playerIndex)
        {
            bool toRet = false;

            if (InputHandle.GetKeyDown(Keys.Down))
                toRet = true;
            else
            {
                int index = m_GamePadTrackers.IndexOfKey(playerIndex);

                Debug.Assert(index >= 0, "Player index " + playerIndex + " was not found in the game pad trackers!");

                if (index >= 0)
                {
                    GamePadState oldState = m_GamePadTrackers.Values[index].oldState;
                    GamePadState currentState = m_GamePadTrackers.Values[index].currentState;

                    if (oldState.IsConnected && currentState.IsConnected)
                        toRet = IsButtonPressed(oldState.DPad.Down, currentState.DPad.Down);
                }
            }

            return toRet;
        }

        // If LEFT is selected from any device
        public bool Left(PlayerIndex playerIndex)
        {
            bool toRet = false;

            if (InputHandle.GetKeyDown(Keys.Left))
                toRet = true;
            else
            {
                int index = m_GamePadTrackers.IndexOfKey(playerIndex);

                Debug.Assert(index >= 0, "Player index " + playerIndex + " was not found in the game pad trackers!");

                if (index >= 0)
                {
                    GamePadState oldState = m_GamePadTrackers.Values[index].oldState;
                    GamePadState currentState = m_GamePadTrackers.Values[index].currentState;

                    if (oldState.IsConnected && currentState.IsConnected)
                        toRet = IsButtonPressed(oldState.DPad.Left, currentState.DPad.Left);
                }
            }

            return toRet;
        }

        // If RIGHT is selected from any device
        public bool Right(PlayerIndex playerIndex)
        {
            bool toRet = false;

            if (InputHandle.GetKeyDown(Keys.Right))
                toRet = true;
            else
            {
                int index = m_GamePadTrackers.IndexOfKey(playerIndex);

                Debug.Assert(index >= 0, "Player index " + playerIndex + " was not found in the game pad trackers!");

                if (index >= 0)
                {
                    GamePadState oldState = m_GamePadTrackers.Values[index].oldState;
                    GamePadState currentState = m_GamePadTrackers.Values[index].currentState;

                    if (oldState.IsConnected && currentState.IsConnected)
                        toRet = IsButtonPressed(oldState.DPad.Right, currentState.DPad.Right);
                }
            }

            return toRet;
        }*/

        /*public bool IsGamePadConnected(PlayerIndex playerIndex)
        {
            bool toRet = false;

            int index = m_GamePadTrackers.IndexOfKey(playerIndex);

            Debug.Assert(index >= 0, "Player index " + playerIndex + " was not found in the game pad trackers!");

            if (index >= 0)
            {
                GamePadState currentState = m_GamePadTrackers.Values[index].currentState;

                // Check if it is connected
                toRet = currentState.IsConnected;
            }

            return toRet;
        }*/

        /*public Vector2 LeftStick(PlayerIndex playerIndex)
        {
            Vector2 toRet = new Vector2(0, 0);

            int index = m_GamePadTrackers.IndexOfKey(playerIndex);

            Debug.Assert(index >= 0, "Player index " + playerIndex + " was not found in the game pad trackers!");

            if (index >= 0)
            {
                GamePadState currentState = m_GamePadTrackers.Values[index].currentState;

                if (currentState.IsConnected)
                    toRet = currentState.ThumbSticks.Left;
            }

            return toRet;
        }

        public Vector2 RightStick(PlayerIndex playerIndex)
        {
            Vector2 toRet = new Vector2(0, 0);

            int index = m_GamePadTrackers.IndexOfKey(playerIndex);

            Debug.Assert(index >= 0, "Player index " + playerIndex + " was not found in the game pad trackers!");

            if (index >= 0)
            {
                GamePadState currentState = m_GamePadTrackers.Values[index].currentState;

                if (currentState.IsConnected)
                    toRet = currentState.ThumbSticks.Right;
            }

            return toRet;
        }

        public bool LeftBumperDown(PlayerIndex playerIndex)
        {
            bool toRet = false;

            int index = m_GamePadTrackers.IndexOfKey(playerIndex);

            Debug.Assert(index >= 0, "Player index " + playerIndex + " was not found in the game pad trackers!");

            if (index >= 0)
            {
                GamePadState currentState = m_GamePadTrackers.Values[index].currentState;

                if (currentState.IsConnected)
                    toRet = currentState.IsButtonDown(Buttons.LeftShoulder);
            }

            return toRet;
        }

        public bool RightBumperDown(PlayerIndex playerIndex)
        {
            bool toRet = false;

            int index = m_GamePadTrackers.IndexOfKey(playerIndex);

            Debug.Assert(index >= 0, "Player index " + playerIndex + " was not found in the game pad trackers!");

            if (index >= 0)
            {
                GamePadState currentState = m_GamePadTrackers.Values[index].currentState;

                if (currentState.IsConnected)
                    toRet = currentState.IsButtonDown(Buttons.RightShoulder);
            }

            return toRet;
        }

        public bool LeftBumperPressed(PlayerIndex playerIndex)
        {
            bool toRet = false;

            int index = m_GamePadTrackers.IndexOfKey(playerIndex);

            Debug.Assert(index >= 0, "Player index " + playerIndex + " was not found in the game pad trackers!");

            if (index >= 0)
            {
                GamePadState oldState = m_GamePadTrackers.Values[index].oldState;
                GamePadState currentState = m_GamePadTrackers.Values[index].currentState;

                if (oldState.IsConnected && currentState.IsConnected)
                    toRet = IsButtonPressed(oldState.Buttons.LeftShoulder, currentState.Buttons.LeftShoulder);
            }

            return toRet;
        }

        public bool RightBumperPressed(PlayerIndex playerIndex)
        {
            bool toRet = false;

            int index = m_GamePadTrackers.IndexOfKey(playerIndex);

            Debug.Assert(index >= 0, "Player index " + playerIndex + " was not found in the game pad trackers!");

            if (index >= 0)
            {
                GamePadState oldState = m_GamePadTrackers.Values[index].oldState;
                GamePadState currentState = m_GamePadTrackers.Values[index].currentState;

                if (oldState.IsConnected && currentState.IsConnected)
                    toRet = IsButtonPressed(oldState.Buttons.RightShoulder, currentState.Buttons.RightShoulder);
            }

            return toRet;
        }

        public float LeftTrigger(PlayerIndex playerIndex)
        {
            float toRet = 0.0f;

            int index = m_GamePadTrackers.IndexOfKey(playerIndex);

            Debug.Assert(index >= 0, "Player index " + playerIndex + " was not found in the game pad trackers!");

            if (index >= 0)
            {
                GamePadState currentState = m_GamePadTrackers.Values[index].currentState;

                if (currentState.IsConnected)
                    toRet = currentState.Triggers.Left;
            }

            return toRet;
        }

        public float RightTrigger(PlayerIndex playerIndex)
        {
            float toRet = 0.0f;

            int index = m_GamePadTrackers.IndexOfKey(playerIndex);

            Debug.Assert(index >= 0, "Player index " + playerIndex + " was not found in the game pad trackers!");

            if (index >= 0)
            {
                GamePadState currentState = m_GamePadTrackers.Values[index].currentState;

                if (currentState.IsConnected)
                    toRet = currentState.Triggers.Right;
            }

            return toRet;
        }*/

       /* public bool IsAnyInput(PlayerIndex playerIndex)
        {
            if (IsMouseLeftPressed() || IsMouseRightPressed() || IsMouseMiddlePressed())
                return true;

            if (KeysPressed().Length > 0)
                return true;

            // No input so far so check game pads
            int index = (int)playerIndex;

            GamePadState oldState = m_GamePadTrackers.Values[index].oldState;
            GamePadState currentState = m_GamePadTrackers.Values[index].currentState;

            if (currentState.IsConnected)
            {
                if (oldState != currentState)
                    return true; 
            }
            
            return false;

            return InputHandle.IsAnyInput(playerIndex);
        }*/

       // #region Properties
        /*public bool PauseApplication { get { return m_PauseApplication; } set { m_PauseApplication = value; } }
        public bool PauseRender { get { return m_PauseRenderering; } }
        public bool ExitApplication { get { return m_ExitApplication; } set { m_ExitApplication = value; } }
        public Viewport ViewPort { get { return m_ViewPort; } set { m_ViewPort = value; } }*/
       // #endregion
   // }
//}*/
