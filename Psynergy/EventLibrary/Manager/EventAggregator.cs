using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Instrumentation;
using System.Diagnostics;

using System.Threading;

/* Main Library */
using Psynergy;

namespace Psynergy.Events
{
    public class EventAggregator : IEventAggregator
    {
        private readonly object m_ListenerLock = new object();
        protected readonly Dictionary< Type, List<IListener>> m_Listeners = new Dictionary<Type, List<IListener>>();

        private SynchronizationContext m_Context;

        public EventAggregator( SynchronizationContext context )
        {
            m_Context = context;
        }

        public virtual void Subscribe( IListener listener )
        {
            ForEachListenerInterfaceImplementedBy( listener, Subscribe);
        }

        public virtual void UnSubscribe(IListener listener)
        {
            ForEachListenerInterfaceImplementedBy( listener, UnSubscribe );
        }

        private void ForEachListenerInterfaceImplementedBy( IListener listener, Action<Type, IListener> action )
        {
            var listenerTypeName = typeof(IListener).Name;

            foreach ( var interfaceType in listener.GetType().GetInterfaces().Where( i => i.Name.StartsWith(listenerTypeName)))
            {
                Type typeOfEvent = GetEventType( interfaceType );

                if ( typeOfEvent != null )
                    action( typeOfEvent, listener );
            }
        }

        private Type GetEventType( Type type )
        {
            Type toRet = null;

            if ( type.GetGenericArguments().Count() > 0 )
                toRet = type.GetGenericArguments()[0];

            return toRet;
        }

        public bool HasListener(Type typeOfEvent, IListener listener)
        {
            bool toRet = false;

            if (m_Listeners.Count > 0)
            {
                if (m_Listeners.ContainsKey(typeOfEvent))
                {
                    if (m_Listeners[typeOfEvent].Contains(listener))
                        toRet = true;
                }
            }

            return toRet;
        }

        public virtual void Subscribe<T>( IListener<T> listener ) where T : IEvent
        {
            Subscribe( typeof(T), listener );
        }

        protected virtual void Subscribe( Type typeOfEvent, IListener listener )
        {
            lock ( m_ListenerLock )
            {
                // Check if a list already exists for this type of event or not.
                if ( !m_Listeners.ContainsKey(typeOfEvent) )
                    m_Listeners.Add( typeOfEvent, new List<IListener>());

                if ( m_Listeners[typeOfEvent].Contains(listener) )
                    throw new InvalidOperationException( "You're not supposed to register to the same event twice!" );

                // Add the event
                m_Listeners[typeOfEvent].Add(listener);
            }
        }

        public virtual void UnSubscribe<T>(IListener<T> listener) where T : IEvent
        {
            UnSubscribe(typeof(T), listener);
        }

        protected virtual void UnSubscribe(Type typeOfEvent, IListener listener)
        {
            lock ( m_ListenerLock )
            {
                if ( m_Listeners.ContainsKey(typeOfEvent) )
                    m_Listeners[typeOfEvent].Remove(listener);
            }
        }

        public void SendMessage<T>(T message) where T : IEvent
        {
            if ( m_Listeners.ContainsKey(typeof(T)) )
                SendAction(() => All(typeof(T)).CallOnEach<IListener<T>>(x => x.Handle(message)));
        }

        public void SendMessage<T>() where T : IEvent, new()
        {
            SendMessage(new T());
        }

        protected virtual void SendAction(Action action)
        {
            // Uses SynchronizationContext to marshall the call to the UI thread
            m_Context.Send(state => { action(); }, null);
        }

        private IEnumerable<KeyValuePair<Type, List<IListener>>> All()
        {
            IEnumerable<KeyValuePair<Type, List<IListener>>> toRet = null;

            lock (m_ListenerLock)
            {
                toRet = m_Listeners;
            }

            return toRet;
        }

        private IEnumerable<IListener> All(Type type)
        {
            IEnumerable<IListener> toRet = null;

            lock (m_ListenerLock)
            {
                List<IListener> eventSubscribeList = null;
                m_Listeners.TryGetValue( type, out eventSubscribeList );

                //Debug.Assert( (eventSubscribeList != null), "Event subscribe list not found for event type " + type.Name );

                toRet = eventSubscribeList;
            }

            return toRet;
        }
    }
}
