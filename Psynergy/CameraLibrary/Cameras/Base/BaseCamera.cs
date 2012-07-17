using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

/* Main Library */
using Psynergy;

namespace Psynergy.Camera
{
    public enum CameraType
    {
        eCameraType_Default,
        eCameraType_Free,
        eCameraType_ThirdPerson,
        eCameraType_World
    };

    public class BaseCamera : Node, ICamera, IFocusable
    {
        protected enum CameraState
        {
            Enter = 0,
            Active = 1,
            Exit = 2,
            Finished = 3
        };

        // Graphics device
        protected GraphicsDevice m_GraphicsDevice = null;

        protected CameraState m_CameraState = CameraState.Enter;

        protected float m_CameraScale = 1.0f;
        protected bool m_StartTween = false;
        protected bool m_Tween = false;

        protected bool m_AllowMovement = true;
        protected bool m_AllowRotation = true;
        protected bool m_CanChange = true;
        protected CameraType m_CameraType = CameraType.eCameraType_Default;

        public BaseCamera() : base("")
        {
            Velocity = 0.0f;
            MoveSpeed = 0.0f;
        }

        public BaseCamera(String name) : base(name)
        {
            Velocity = 0.0f;
            MoveSpeed = 0.0f;
        }

        public override void Initialise()
        {
            base.Initialise();

            // Default transform to identity matrix
            Transform = Matrix.Identity;



            // Register any camera events
            RegisterEvents();
        }

        protected virtual void RegisterEvents()
        {
        }

        public override void Reset()
        {
            base.Reset();

            m_Tween = m_StartTween;
            m_CameraState = CameraState.Enter;
        }

        public override void Update(GameTime deltaTime)
        {
            base.Update(deltaTime);

            switch ( m_CameraState )
            {
                case CameraState.Enter:
                {
                    UpdateEnter(deltaTime);
                }
                break;

                case CameraState.Active:
                {
                    UpdateActive(deltaTime);
                }
                break;

                case CameraState.Exit:
                {
                    UpdateExit(deltaTime);
                }
                break;

                case CameraState.Finished:
                {
                }
                break;
            }

            // Setup the camera
            SetUpCamera(deltaTime);
        }

        protected virtual void OnEnter()
        {
            m_CameraState = CameraState.Enter;

            // Don't allow change when beginning to exit a camera
            m_CanChange = false;
        }

        protected virtual void UpdateEnter(GameTime deltaTime)
        {
            OnActive();
        }

        protected virtual void OnActive()
        {
            m_CameraState = CameraState.Active;

            // Since this just instantly moves to active state allow change
            m_CanChange = true;
        }

        protected virtual void UpdateActive(GameTime deltaTime)
        {
            if (m_AllowMovement)
            {
                // Update camera input
                Move(deltaTime);
            }

            if (m_AllowRotation)
            {
                // Update camera rotation
                Rotate(deltaTime);
            }
        }

        protected virtual void OnExit()
        {
            m_CameraState = CameraState.Exit;

            // Don't allow change when beginning to exit a camera
            m_CanChange = false;
        }

        protected virtual void UpdateExit(GameTime deltaTime)
        {
            if (m_AllowMovement)
            {
                // Update camera input
                Move(deltaTime);
            }

            if (m_AllowRotation)
            {
                // Update camera rotation
                Rotate(deltaTime);
            }

            if (IsExitFinished())
                OnExitFinished();
        }

        protected virtual bool IsExitFinished()
        {
            return true;
        }

        protected virtual void OnExitFinished()
        {
            m_CameraState = CameraState.Finished;

            // Can change again
            m_CanChange = true;
        }

        protected virtual void Move(GameTime deltaTime)
        {
        }

        protected virtual void Rotate(GameTime deltaTime)
        {
        }

        public virtual void SetUpCamera(GameTime deltaTime)
        {
        }

        // Used to determine whether it is within the view or not
        public virtual bool IsInView(Node sprite)
        {
            // In View
            return true;
        }

        public virtual void SetFocus(IFocusable focus)
        {
            if ( IFocus != focus )
                IFocus = focus;

            if ( IFocus != null )
                IFocus.Focused = true;
        }

        public virtual void SetInstantFocus(IFocusable focus)
        {
            SetFocus(focus);
            m_Tween = false;
        }

        public virtual void OnChangedTo(BaseCamera previousCamera)
        {
            OnEnter();
        }

        public virtual void OnChange()
        {
            OnExit();
        }

        #region Properties
        public GraphicsDevice GraphicsDevice { get { return m_GraphicsDevice; } set { m_GraphicsDevice = value; } }
        public float CameraScale { get; set; }
        public virtual Matrix Transform { get; set; }
        public bool StartTween { get { return m_StartTween; } set { m_StartTween = value; } }
        public bool Tween { get { return m_Tween; } set { m_Tween = value; } }
        public float MoveSpeed { get; set; }
        public float Velocity { get; set; }

        public bool RequiresFocus { get; set; }
        public IFocusable IFocus { get; set; }

        public bool AllowMovement { get { return m_AllowMovement; } set { m_AllowMovement = value; } }
        public bool AllowRotation { get { return m_AllowRotation; } set { m_AllowRotation = value; } }
        public bool CanChange { get { return m_CanChange; } }
        public CameraType CameraType { get { return m_CameraType; } }
        #endregion
    }
}
