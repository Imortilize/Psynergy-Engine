using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Instrumentation;

using Microsoft.Xna.Framework;

/* Main Library */
using Psynergy;

namespace Psynergy.Events
{
    public class SelectObjectEvent : IEvent
    {
        private Ray m_CastRay;

        public SelectObjectEvent(Ray ray)
        {
            m_CastRay = ray;
        }

        public void Fire()
        {
            EventManager.Instance.SendMessage(this);
        }

        public Ray CastedRay { get { return m_CastRay; } }
    }

}
