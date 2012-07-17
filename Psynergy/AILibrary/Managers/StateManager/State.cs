using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Psynergy.AI
{
    public class State<T> : AbstractState<T>
    {
        // Variables
        protected bool m_IsClosing = false;
        protected bool m_Closed = true;
        protected bool m_Finished = true;

        public State() : base()
        {
        }

        public override void Initialise()
        {
        }

        public override void Reset()
        {
            m_IsClosing = false;
            m_Closed = true;
            m_Finished = true;
        }

        public override void OnEnter(T objectRef)
        {
            m_IsClosing = false;
            m_Closed = false;
            m_Finished = false;
        }

        public override void Update(GameTime deltaTime, T objectRef)
        {
            // Check if it is closing, if so run a close check function
            if (m_IsClosing)
            {
                if (CloseChecks())
                {
                    m_IsClosing = false;
                    m_Closed = true;
                }
            }
        }

        public override void Render(GameTime deltaTime, T objectRef)
        {
        }

        protected virtual bool CloseChecks()
        {
            return true;
        }

        public override void OnExit(T objectRef)
        {
            m_IsClosing = true;
        }

        public virtual void OnFinish()
        {
            m_Finished = true;
        }

        public bool IsClosed()
        {
            return m_Closed;
        }

        public bool IsFinished()
        {
            return m_Finished;
        }
    }
}
