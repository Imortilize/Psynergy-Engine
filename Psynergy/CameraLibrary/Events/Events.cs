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
        private GameObject m_FocusObject;

        public CameraFocusEvent(GameObject focusObject)
        {
            m_FocusObject = focusObject;
        }

        public void Fire()
        {
            EventManager.Instance.SendMessage(this);
        }

        public GameObject FocusObject { get { return m_FocusObject; } }
    }
}
