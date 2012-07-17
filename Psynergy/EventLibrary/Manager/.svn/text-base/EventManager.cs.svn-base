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

        public void Subscribe<T>(IListener<T> listener) where T : IEvent
        {
            if ( !m_EventAggregator.HasListener(typeof(T), listener) )
                m_EventAggregator.Subscribe(listener);
        }

        public void UnSubscribe<T>(IListener<T> listener) where T : IEvent
        {
            m_EventAggregator.UnSubscribe(listener);
        }

        public void SendMessage<T>(T message) where T : IEvent
        {
            m_EventAggregator.SendMessage(message);
        }

        public override void Update(GameTime deltaTime)
        {
            base.Update(deltaTime);
        }

        public override void Render(GameTime deltaTime)
        {
            base.Render(deltaTime);
        }
    }
}
