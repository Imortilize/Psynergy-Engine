using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Instrumentation;

using Microsoft.Xna.Framework;

/* Event Library */
using Psynergy.Events;

namespace XnaGame
{
    public class PlayerWonEvent : IEvent
    {
        private PlayerIndex m_Index;

        public PlayerWonEvent(PlayerIndex index)
        {
            m_Index = index;
        }

        public void Fire()
        {
            EventManager.Instance.SendMessage(this);
        }

        public PlayerIndex Index { get { return m_Index; } }
    }

    public class FirstQuestionPassedEvent : IEvent
    {
        public FirstQuestionPassedEvent()
        {
        }

        public void Fire()
        {
            EventManager.Instance.SendMessage(this);
        }
    }

}
