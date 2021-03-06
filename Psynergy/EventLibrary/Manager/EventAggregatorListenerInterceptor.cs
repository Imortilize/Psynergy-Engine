﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Win32;

/* Main Library */
using Psynergy;

namespace Psynergy.Events
{
    public class EventAggregatorListenerInterceptor : TypeInterceptor
    {
        public object Process(object target, IContext context)
        {
            context.GetInstance<IEventAggregator>().Subscribe((IListener)target);

            return target;
        }

        public bool MatchesType(Type type)
        {
            return type.GetInterfaces().Contains(typeof(IListener));
        }
    }

    /*public class EventAggregationRegistry : Reg
    {
        public EventAggregationRegistry()
        {
            //For<IEventAggregator>().Singleton().Use<EventAggregator>();

            
           // RegisterInterceptor(new EventAggregatorListenerInterceptor());
            int spug = 0;
        }
    }*/
}
