using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Instrumentation;

using Microsoft.Xna.Framework;

/* Main Library */
using Psynergy;

/* Events Library */
using Psynergy.Events;

namespace Psynergy.Camera
{
    public class CameraFocusEvent : IEvent
    {
        private Node m_FocusObject;

        public CameraFocusEvent(Node focusObject)
        {
            m_FocusObject = focusObject;
        }

        public void Fire()
        {
            EventManager.Instance.SendMessage(this);
        }

        public Node FocusObject { get { return m_FocusObject; } }
    }
}
