using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Instrumentation;

using Microsoft.Xna.Framework;

/* Main Library */
using Psynergy;

/* Event Library */
using Psynergy.Events;

namespace Psynergy.Input
{
    public class PauseGameEvent : IEvent
    {
        private bool m_Pause = false;

        public PauseGameEvent(bool pause)
        {
            m_Pause = pause;
        }

        public void Fire()
        {
            EventManager.Instance.SendMessage(this);
        }

        public bool Pause { get { return m_Pause; } }
    }
}
