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

namespace Psynergy.Camera
{
    public class Camera2D : BaseCamera, ICamera2D
    {
        protected float m_Rotation = 0.0f;
        protected float m_StartRotation = 0.0f;

        protected float m_ViewportHeight = 0.0f;
        protected float m_ViewportWidth = 0.0f;

        private Point m_MovementBoundary = new Point(100, 100);

        public Camera2D() : base("")
        {
        }

        public Camera2D(String name) : base(name)
        {
        }

        public override void Initialise()
        {
            /*Debug.Assert( RenderManager.Instance.GraphicsDevice != null, "Render manager graphics device is null when it should be!" );

            if ( RenderManager.Instance.GraphicsDevice != null )
            {
                m_ViewportWidth = RenderManager.Instance.GraphicsDevice.Viewport.Width;
                m_ViewportHeight = RenderManager.Instance.GraphicsDevice.Viewport.Height;
            }*/

            ScreenCenter = new Vector2(m_ViewportWidth / 2, m_ViewportHeight / 2);
            transform.Scale = new Vector3(m_CameraScale);
            MoveSpeed = 1.25f;

            base.Initialise();

            // Set that it does required focus
            RequiresFocus = true;
        }

        public override void Reset()
        {
            base.Reset();

            Rotation = m_StartRotation;
        }

        public override void Update( GameTime deltaTime )
        {
            base.Update(deltaTime);

            // Any code that needs to happen after setting up the camera...
        }

        public override void SetUpCamera(GameTime deltaTime)
        {
            // Move the Camera to the position that it needs to go
            var delta = (float)deltaTime.ElapsedGameTime.TotalSeconds;

            // Get pos
            Vector3 pos = transform.Position;

            // If the camera has a focus 
            if (Focus != null)
            {
                // If using tween properties
                if (m_Tween)
                {
                    pos.X += (((Focus.transform.Position.X + (Focus.Width / 2)) - pos.X) * MoveSpeed * delta);
                    pos.Y += (((Focus.transform.Position.Y + (Focus.Height / 2)) - pos.Y) * MoveSpeed * delta);
                }
                else
                {
                    pos.X = ((Focus.transform.Position.X) + (Focus.Width / 2));
                    pos.Y = ((Focus.transform.Position.Y) + (Focus.Height / 2));
                }
            }
            else
            {
                pos.X += (-pos.X * MoveSpeed * delta);
                pos.Y += (-pos.Y * MoveSpeed * delta);
            }

            Origin = new Vector2( 0, 0 );

            // Create the Transform used by any
            // spritebatch process
            transform.WorldMatrix = Matrix.Identity *
                                    Matrix.CreateTranslation(-pos.X, -pos.Y, 0) *
                                    Matrix.CreateRotationZ(Rotation) *                // Use rotation X as we only need 1 value in 2d cameras
                                    Matrix.CreateTranslation(Origin.X, Origin.Y, 0) *
                                    Matrix.CreateScale(transform.Scale);

            // Set position back
            transform.Position = pos;
        }

        // Used to determine whether it is within the view or not
        public override bool IsInView(GameObject sprite)
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

        #region Properties
        public Vector2 Origin { get; set; }
        public Vector2 ScreenCenter { get; protected set; }
        public float Rotation { get { return m_Rotation; } set { m_Rotation = value; } }
        public IFocusable2D Focus { get { return (IFocus as IFocusable2D); } set { IFocus = value; } }
        #endregion
    }
}
