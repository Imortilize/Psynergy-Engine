using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

/* Graphics Library */
using Psynergy.Graphics;

namespace Psynergy.Menus
{
    public class UIObject : SpriteNode
    {
        public enum ActiveState
        {
            eInActive = 0,
            eIn = 1,
            eIdle = 2,
            eOut = 3,
            eFocus = 4,
            eUnFocus = 5,
            eRemove = 6
        };

        // Whether to remove the object from the UIManager or not
        protected bool m_RemoveUIObject = false;

        // UIObject's active state
        protected ActiveState m_ActiveState = ActiveState.eInActive;
        protected ActiveState m_PreviousState = ActiveState.eInActive;

        // UIObject effect
        protected UIObjectEffect m_Effect = null;

        public UIObject() : base()
        {
            m_RootFolderName = "Textures/UI/";
        }

        public UIObject(String name) : base(name)
        {
            m_RootFolderName = "Textures/UI/";
        }

        public UIObject(String name, String assetName, Vector3 position) : base(name, assetName, position)
        {
            m_RootFolderName = "Textures/UI/";
        }

        public override void  Initialise()
        {
 	         base.Initialise();

            // So they render above backgrounds still
             m_RenderDepth = 0.9f;
        }

        public override void Reset()
        {
            base.Reset();

            // Remove UIObject
            UIManager.Instance.RemoveUIObject(this);

            m_RemoveUIObject = false;

            if (m_Effect != null)
                m_Effect.Reset();

            m_ActiveState = ActiveState.eInActive;
        }

        public override void Load()
        {
            base.Load();
        }

        public override void UnLoad()
        {
            base.UnLoad();
        }

        public override void Update(GameTime deltaTime)
        {
            base.Update(deltaTime);

            switch (m_ActiveState)
            {
                case UIObject.ActiveState.eInActive:
                    {
                        // Do nothing
                    }
                    break;
                case UIObject.ActiveState.eIn:
                    {
                        // Run in code
                        OnIn(deltaTime);
                    }
                    break;
                case UIObject.ActiveState.eIdle:
                    {
                        // Run in code
                        OnIdle(deltaTime);
                    }
                    break;
                case UIObject.ActiveState.eOut:
                    {
                        // Run in code
                        OnOut(deltaTime);
                    }
                    break;
                case UIObject.ActiveState.eFocus:
                    {
                        // Run in code
                        OnFocus(deltaTime);
                    }
                    break;
                case UIObject.ActiveState.eUnFocus:
                    {
                        // Run in code
                        OnUnFocus(deltaTime);
                    }
                    break;
                case UIObject.ActiveState.eRemove:
                    {
                        Reset();
                    }
                    break;
            }

            // Run effect if it exists
            UpdateUIEffect(deltaTime);
        }

        private void UpdateUIEffect(GameTime deltaTime)
        {
            if (m_Effect != null)
                m_Effect.Update(deltaTime);
        }

        protected virtual void OnIn(GameTime deltaTime)
        {
        }

        protected virtual void OnIdle(GameTime deltaTime)
        {
        }

        protected virtual void OnOut(GameTime deltaTime)
        {
        }

        protected virtual void OnFocus(GameTime deltaTime)
        {
        }

        protected virtual void OnUnFocus(GameTime deltaTime)
        {
        }

        public override void Render(GameTime deltaTime)
        {
            base.Render(deltaTime);
        }

        public virtual bool RemoveObject()
        {
            return m_RemoveUIObject;
        }

        public virtual void OnShow()
        {
            m_ActiveState = ActiveState.eIn;

            if (m_Effect != null)
                m_Effect.OnEnter();
        }

        public virtual void OnRemove()
        {
            m_ActiveState = ActiveState.eOut;

            if (m_Effect != null)
                m_Effect.OnExit();
            else
                UIManager.Instance.RemoveUIObject(this);
        }

        public void SetActiveState(ActiveState activeState)
        {
            SetPreviousState();

            m_ActiveState = activeState;
        }

        public void SetUIEffect(UIObjectEffect effect)
        {
            m_Effect = effect;

            if (m_Effect != null)
            {
                m_Effect.Initialise();
                m_Effect.Load();
                m_Effect.Reset();
            }
        }

        public bool IsIdle()
        {
            return (m_ActiveState == ActiveState.eIdle);
        }

        public void UnFocus()
        {
            SetPreviousState();

            m_ActiveState = ActiveState.eUnFocus;
        }

        public void Focus()
        {
            SetPreviousState();

            m_ActiveState = ActiveState.eFocus;
        }

        private void SetPreviousState()
        {
            m_PreviousState = m_ActiveState;
        }

        public void GotoPreviousState()
        {
            m_ActiveState = m_PreviousState;
        }

        #region Property Set / Gets
        public ActiveState CurrentActiveState { get { return m_ActiveState; } set { m_ActiveState = value; } }
        public bool Remove { get { return m_RemoveUIObject; } set { m_RemoveUIObject = true; } }
        #endregion
    }
}
