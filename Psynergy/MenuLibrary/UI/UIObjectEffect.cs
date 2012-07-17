using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

/* Main Library */
using Psynergy;

namespace Psynergy.Menus
{
    public class UIObjectEffect : GameObject
    {
        // GameObject related to
        protected UIObject m_ObjReference = null;

        public UIObjectEffect() : base()
        {
        }

        public override void Initialise()
        {
            base.Initialise();
        }

        public override void Load()
        {
            base.Load();
        }

        public override void Reset()
        {
            base.Reset();
        }

        public override void Update(GameTime deltaTime)
        {
            base.Update(deltaTime);

            if (m_ObjReference != null)
            {
                UIObject.ActiveState activeState = m_ObjReference.CurrentActiveState;

                switch (activeState)
                {
                    case UIObject.ActiveState.eInActive:
                        {
                            // Do nothing
                            //m_ObjReference.Remove = true;
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
                            // Remove UIObject
                            UIManager.Instance.RemoveUIObject(m_ObjReference);

                            // Change to inactive state
                            m_ObjReference.SetActiveState(UIObject.ActiveState.eInActive);
                        }
                        break;
                }
            }
        }

        public virtual void OnEnter()
        {
        }

        public virtual void OnExit()
        {
        }

        protected virtual void OnIn(GameTime deltaTime)
        {
            m_ObjReference.SetActiveState(UIObject.ActiveState.eIdle);
        }

        protected virtual void OnIdle(GameTime deltaTime)
        {
        }

        protected virtual void OnOut(GameTime deltaTime)
        {
            m_ObjReference.SetActiveState(UIObject.ActiveState.eRemove);
        }

        protected virtual void OnFocus(GameTime deltaTime)
        {
            m_ObjReference.SetActiveState(UIObject.ActiveState.eIdle);
        }

        protected virtual void OnUnFocus(GameTime deltaTime)
        {
            m_ObjReference.SetActiveState(UIObject.ActiveState.eIdle);
        }

        public void SetUIObject(UIObject uiObject)
        {
            m_ObjReference = uiObject;

            if (m_ObjReference != null)
                m_ObjReference.SetUIEffect(this);
        }
    }
}
