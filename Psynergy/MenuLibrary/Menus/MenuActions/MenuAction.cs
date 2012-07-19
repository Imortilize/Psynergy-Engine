using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

/* Main Library */
using Psynergy;

/* Sound Library */
using Psynergy.Sound;

/* Input Library */
using Psynergy.Input;

namespace Psynergy.Menus
{
    public class MenuAction : GameObject, IMenuAction, IRegister<MenuAction>
    {
        protected bool m_RunAction = false;

        protected List<String> m_ActionTypes = new List<String>();

        // Sound reference
        protected String m_SoundName = "";

        public MenuAction()
        {
        }

        public MenuAction(String name)
            : base(name)
        {
        }

        public override void Initialise()
        {
            m_RunAction = false;
        }

        public void SetKeyPress( String inputType )
        {
            if (inputType == "select")
                m_ActionTypes.Add("Submit");
            else if (inputType == "return")
                m_ActionTypes.Add("Return");
            else if (inputType == "click")
                m_ActionTypes.Add("Click");
        }

        public bool CheckAction()
        {
            m_RunAction = false;

            if (m_ActionTypes.Count > 0)
            {
                foreach (String action in m_ActionTypes)
                {
                    if (action == "Submit")
                        m_RunAction = InputManager.Instance.Submit( PlayerIndex.One );
                    else if (action == "Return")
                        m_RunAction = InputManager.Instance.Return( PlayerIndex.One );
                    else if (action == "Click")
                        m_RunAction = InputManager.Instance.Click(PlayerIndex.One);
                }
            }

            return m_RunAction;
        }

        public virtual void RunAction()
        {
            m_RunAction = false;

            // Play sound
            if (m_SoundName != "")
                SoundManager.Instance.PlaySound(m_SoundName);
        }

        #region Property Set / Gets
        public String Sound { get { return m_SoundName; } set { m_SoundName = value; } } 
        #endregion
    }
}
