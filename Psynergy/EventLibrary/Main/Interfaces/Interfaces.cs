using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Instrumentation;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Psynergy.Events
{
    public interface IContext
    {
        // Gets a reference to the buildstack for this build session
        //BuildStack BuildStack { get; }

        // The concrete type of the immediate parent object in the object graph
        Type ParentType { get; }

        // Get the object type T that is valid for this build session
        T GetInstance<T>();

        // Get the object of type T that is valid for this build session by name
        T GetInstance<T>(String name);

        // Gets the root "frame" of the object request
        //BuildFrame Root { get; }

        // The requested instance name of the object graph
        String RequestedName { get; }

        // Register a default object for the given PluginType that will be used 
        // throughout the rest of teh current objet request
        void RegisterDefault(Type plugintype, object defaultObject);

        // Same as GetInstance, but can gracefully return null if the Type
        // does no already exist
        T TryGetInstance<T>() where T : class;
            
        // Same as GetInstance(name), but can gracefully return null if the Type
        // and name does no already exist
        T TryGetInstance<T>(String name) where T : class;
    }

    /* Event aggregator interface */
    public interface IEventAggregator
    {
        void SendMessage<T>(T message) where T : IEvent;
        void SendMessage<T>() where T : IEvent, new();

        void Subscribe(IListener listener);
        void UnSubscribe(IListener listener);

        void Subscribe<T>(IListener<T> listener) where T : IEvent;
        void UnSubscribe<T>(IListener<T> listener) where T : IEvent;
    }

    public interface IListener
    {
    }

    public interface IListener<T> : IListener where T : IEvent
    {
        void Handle(T message);
    }

    public interface InstanceInterceptor
    {
        object Process(object target, IContext context);
    }

    public interface TypeInterceptor : InstanceInterceptor
    {
        // Does this type interceptor apply to the given type?
        bool MatchesType(Type type);
    }
}
