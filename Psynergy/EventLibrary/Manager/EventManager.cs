using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Management.Instrumentation;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

/* Main Library */
using Psynergy;

namespace Psynergy.Events
{
    public class EventManager : Singleton<EventManager>
    {
        private EventAggregator m_EventAggregator;

        public EventManager()
        {
        }

        public EventManager(EventAggregator eventAggregator)
        {
            m_EventAggregator = eventAggregator;
        }

        public override void Initialise()
        {
            base.Initialise();

            // Auto register events
            AutoRegisterEvents();
        }

        public override void Reset()
        {
            base.Reset();
        }

        public override void Load()
        {
            base.Load();
        }

        public override void UnLoad()
        {
            base.UnLoad();
        }

        public void Subscribe(IListener listener)
        {
            m_EventAggregator.Subscribe(listener);
        }

        public void UnSubscribe(IListener listener)
        {
            m_EventAggregator.UnSubscribe(listener);
        }

        public void SendMessage<T>(T message) where T : IEvent
        {
            m_EventAggregator.SendMessage(message);
        }

        #region Auto Registeration of event subscribers
        private void AutoRegisterEvents()
        {
            List<Type> types = Factory.Instance.GetTypesUsingInterface("IListener`1", true);

            foreach (Type type in types)
            {
                // Create an instance of the object type
                IListener dummyObj = (Factory.Instance.CreateInstance(type) as IListener);

                // Register the event subscriber
                Subscribe(dummyObj);
            }
        }
        #endregion
    }
}
