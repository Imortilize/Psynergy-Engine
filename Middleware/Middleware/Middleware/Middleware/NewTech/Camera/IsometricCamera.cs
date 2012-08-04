using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

/* Main Library */
using Psynergy;
using Psynergy.Input;
using Psynergy.Graphics;
using Psynergy.Camera;

namespace Middleware
{
    public class IsometricCamera : BaseCamera, ICamera2D, IRegister<IsometricCamera>
    {
        protected float m_Rotation = 0.0f;
        protected float m_StartRotation = 0.0f;

        // Screen size
        private Vector2 m_ViewPort = Vector2.Zero;

        // World size
        private Vector2 m_WorldSize = Vector2.Zero;

        // Display offset
        private Vector2 m_DisplayOffset = Vector2.Zero;

        // Reference to tile map
        private TileMap m_TileMap = null;

        public IsometricCamera() : base("")
        {
        }

        public IsometricCamera(String name) : base(name)
        {
        }

        public override void Initialise()
        {
            Debug.Assert( RenderManager.Instance.GraphicsDevice != null, "Render manager graphics device is null when it should be!" );

            if ( RenderManager.Instance.GraphicsDevice != null )
            {
                m_ViewPort.X = RenderManager.Instance.GraphicsDevice.Viewport.Width;
                m_ViewPort.Y = RenderManager.Instance.GraphicsDevice.Viewport.Height;
            }

            // Set camera scale
            Scale = new Vector3(m_CameraScale, m_CameraScale, m_CameraScale);
            Origin = new Vector2(0, 0);

            base.Initialise();

            // Set that it does required focus
            RequiresFocus = false;
        }

        public override void Reset()
        {
            base.Reset();

            // Set initial rotation
            Rotation = m_StartRotation;
        }

        public override void Update(GameTime deltaTime)
        {
            base.Update(deltaTime);

            // Any code that needs to happen after setting up the camera...
        }

        public override void SetUpCamera(GameTime deltaTime)
        {
            // Move the Camera to the position that it needs to go
            var delta = (float)deltaTime.ElapsedGameTime.TotalSeconds;

            // If the camera has a focus 
           /* if (Focus != null)
            {
                // If using tween properties
                if (m_Tween)
                {
                    PosX += (((Focus.Position.X + (Focus.Width / 2)) - PosX) * MoveSpeed * delta);
                    PosY += (((Focus.Position.Y + (Focus.Height / 2)) - PosY) * MoveSpeed * delta);
                }
                else
                {
                    PosX = ((Focus.Position.X) + (Focus.Width / 2));
                    PosY = ((Focus.Position.Y) + (Focus.Height / 2));
                }
            }
            else
            {
                PosX += (-PosX * MoveSpeed * delta);
                PosY += (-PosY * MoveSpeed * delta);
            }*/

            InputManager input = InputManager.Instance;

            if ((input != null) && (m_TileMap != null))
            {
                if (input.KeyDown(Keys.Left))
                {
                    //PosX = MathHelper.Clamp((PosX - 2), 0, (testX * tileWidth));
                    Move(new Vector2(-2, 0));
                }

                if (input.KeyDown(Keys.Right))
                {
                    //PosX = MathHelper.Clamp((PosX + 2), 0, (testX * tileWidth));
                    Move(new Vector2(2, 0));
                }

                if (input.KeyDown(Keys.Up))
                {
                    //PosY = MathHelper.Clamp((PosY - 2), 0, (testY * tileHeight));
                    Move(new Vector2(0, -2));
                }

                if (input.KeyDown(Keys.Down))
                {
                    //PosY = MathHelper.Clamp((PosY + 2), 0, (testY * tileHeight));
                    Move(new Vector2(0, 2));
                }
            }

            //Origin = new Vector2(0, 0);

            // Create the Transform used by any
            // spritebatch process
            Transform = Matrix.Identity *
                        Matrix.CreateTranslation(-PosX, -PosY, 0) *
                        Matrix.CreateRotationZ(Rotation) *                // Use rotation X as we only need 1 value in 2d cameras
                        Matrix.CreateTranslation(Origin.X, Origin.Y, 0) *
                        Matrix.CreateScale(Scale);
        }

        // Used to determine whether it is within the view or not
        public override bool IsInView(Node sprite)
        {
            return false;
            // If the object is not within the horizontal bounds of the screen

            //if (((sprite.PosX + sprite.Width) < (PosX - Origin.X)) || ((sprite.PosX) > (PosX + Origin.X)))
            //return false;

            // If the object is not within the vertical bounds of the screen
            //if (((sprite.PosY + sprite.Height) < (PosY - Origin.Y)) || ((sprite.PosY) > (PosY + Origin.Y)))
            //return false;

            // In View
            //return true;
        }

        public Vector2 WorldToScreen(Vector2 worldPosition)
        {
            return (worldPosition - Location + m_DisplayOffset);
        }

        public Vector2 ScreenToWorld(Vector2 screenPosition)
        {
            return (screenPosition + Location - m_DisplayOffset);
        }

        private void Move(Vector2 offset)
        {
            Location += offset;
        }

        #region Properties
        public Vector2 Origin { get; set; }
        public Vector2 ScreenCenter { get; protected set; }
        public float Rotation { get { return m_Rotation; } set { m_Rotation = value; } }
        public IFocusable2D Focus { get { return (IFocus as IFocusable2D); } set { IFocus = value; } }
        public TileMap TileMap 
        { 
            set 
            { 
                m_TileMap = value;

                if (m_TileMap != null)
                {
                    // Set world size
                    m_WorldSize.X = ((m_TileMap.GridSize.X - 2) * m_TileMap.TileStep.X);
                    m_WorldSize.Y = ((m_TileMap.GridSize.Y - 2) * m_TileMap.TileStep.Y);

                    // Set display offset
                    m_DisplayOffset = m_TileMap.BaseOffset;
                }
            } 
        }

        public Vector2 Location
        {
            get 
            {
                return GetPos2D();
            }
            set
            {
                PosX = MathHelper.Clamp(value.X, 0f, (m_WorldSize.X - m_ViewPort.X));
                PosY = MathHelper.Clamp(value.Y, 0f, (m_WorldSize.Y - m_ViewPort.Y));
            }
        }
        #endregion
    }
}
