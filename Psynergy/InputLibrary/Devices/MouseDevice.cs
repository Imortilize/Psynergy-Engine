using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Psynergy.Input      
{
    public class MouseInput
    {
        #region Enums
        public enum InputType
        {
            leftButton,
            rightButton,
            middleButton
        };
        #endregion

        #region Fields
        private InputType m_InputType = InputType.leftButton;
        private bool m_Pressed = false;
        private bool m_Released = false;
        private bool m_Down = false;
        #endregion

        #region Constructor
        public MouseInput(InputType type)
        {
            m_InputType = type;
        }
        #endregion

        #region Functions
        public void Reset()
        {
            m_Pressed = false;
            m_Released = false;
            m_Down = false;
        }

        public void Update(ButtonState state)
        {
            bool buttonPressed = (state == ButtonState.Pressed);
            bool buttonReleased = (state == ButtonState.Released);

            // Update click detections but only when button is clicked
            m_Pressed = (buttonPressed && !m_Down);
            m_Released = buttonReleased;
           
            // These ones always return true if the button is down
            m_Down = buttonPressed;
        }
        #endregion

        #region Properties
        public InputType Type { get { return m_InputType; } }
        public bool Pressed { get { return m_Pressed; } }
        public bool Released { get { return m_Released; } }
        public bool Down { get { return m_Down; } }
        #endregion
    };

    public class MouseDevice : InputDevice
    {
        #region Enums
        #endregion

        #region Structs
        #endregion

        #region Fields
        private Viewport m_ViewPort;

        // Mouse values
        private MouseState m_LastMouseState;
        private MouseState m_CurrentMouseState;

        // Mouse Deltas
        private float m_MouseDeltaX = 0.0f;
        private float m_MouseDeltaY = 0.0f;
        private float m_MouseWheelDelta = 0.0f;

        // New Input List
        private List<MouseInput> m_Inputs = new List<MouseInput>();
        #endregion

        #region Constructor
        public MouseDevice() : base()
        {
            // Get the mouse state on load
            m_LastMouseState = Mouse.GetState();
            m_CurrentMouseState = Mouse.GetState();

            // Add inputs
            AddInput(new MouseInput(MouseInput.InputType.leftButton));
            AddInput(new MouseInput(MouseInput.InputType.rightButton));
            AddInput(new MouseInput(MouseInput.InputType.middleButton));
        }
        #endregion

        #region Functions
        public override void Update(GameTime deltaTime)
        {
            if (m_CurrentMouseState != null)
            {
                Viewport viewPort = m_ViewPort;
                MouseState newMouseState = Mouse.GetState();

                if ((newMouseState.X >= 0) && (newMouseState.Y >= 0) && (newMouseState.X <= viewPort.Width) && (newMouseState.Y <= viewPort.Height))
                {
                    if (m_LastMouseState != m_CurrentMouseState)
                        m_LastMouseState = m_CurrentMouseState;

                    m_CurrentMouseState = newMouseState;

                    // Clamp within boundaries of viewport to prevent crashing
                    float mouseX = MathHelper.Clamp(m_CurrentMouseState.X, 0, viewPort.Width);
                    float mouseY = MathHelper.Clamp(m_CurrentMouseState.Y, 0, viewPort.Height);

                    // Calculate the mouse deltas
                    m_MouseDeltaX = (((float)m_LastMouseState.X - (float)mouseX) * (float)deltaTime.ElapsedGameTime.TotalSeconds);
                    m_MouseDeltaY = (((float)m_LastMouseState.Y - (float)mouseY) * (float)deltaTime.ElapsedGameTime.TotalSeconds);
                    m_MouseWheelDelta = (((float)m_LastMouseState.ScrollWheelValue - (float)m_CurrentMouseState.ScrollWheelValue) * (float)deltaTime.ElapsedGameTime.TotalSeconds);

                    // Update mouse input buttons
                    UpdateInput(MouseInput.InputType.leftButton, m_CurrentMouseState.LeftButton);
                    UpdateInput(MouseInput.InputType.rightButton, m_CurrentMouseState.RightButton);
                    UpdateInput(MouseInput.InputType.middleButton, m_CurrentMouseState.MiddleButton);
                }
                else
                {
                    // Reset mouse deltas
                    m_MouseDeltaX = 0;
                    m_MouseDeltaY = 0;
                    m_MouseWheelDelta = 0;

                    // Reset inputs
                    for (int i = 0; i < m_Inputs.Count; i++)
                        m_Inputs[i].Reset();
                }
            }
        }

        public bool GetMouse(int index)
        {
            bool toRet = false;

            MouseInput input = GetInput(index);
            if (input != null)
                toRet = input.Down;

            return toRet;
        }

        public bool GetMouseDown(int index)
        {
            bool toRet = false;

            MouseInput input = GetInput(index);
            if (input != null)
                toRet = input.Pressed;

            return toRet;
        }

        public bool GetMouseUp(int index)
        {
            bool toRet = false;

            MouseInput input = GetInput(index);
            if (input != null)
                toRet = input.Released;

            return toRet;
        }

        public override bool IsConnected()
        {
            bool toRet = false;

            if (m_CurrentMouseState != null)
                toRet = true;

            return toRet;
        }
        #endregion

        #region Helper Functions
        private void AddInput(MouseInput input)
        {
            m_Inputs.Add(input);
        }

        private MouseInput GetInput(int index)
        {
            MouseInput input = null;

            if (index < m_Inputs.Count)
                input = m_Inputs[index];

            return input;
        }

        private void UpdateInput(MouseInput.InputType type, ButtonState state)
        {
            MouseInput input = GetInput((int)type);
            if (input != null)
                input.Update(state);
        }
        #endregion

        #region Properties
        public Vector2 MousePosition
        {
            get
            {
                Viewport viewPort = m_ViewPort;

                // Clamp within boundaries of viewport to prevent crashing
                float mouseX = MathHelper.Clamp(m_CurrentMouseState.X, 0, viewPort.Width);
                float mouseY = MathHelper.Clamp(m_CurrentMouseState.Y, 0, viewPort.Height);

                return new Vector2(mouseX, mouseY);
            }
        }

        public Vector3 MouseDelta { get { return new Vector3(m_MouseDeltaX, m_MouseDeltaY, m_MouseWheelDelta); } }
        public Viewport ViewPort { set { m_ViewPort = value; } }
        #endregion
    }
}
