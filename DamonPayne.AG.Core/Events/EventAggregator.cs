using System;
using System.Collections.Generic;
using System.Reflection;
using DamonPayne.AG.Core.DataTypes;

namespace DamonPayne.AG.Core.Events
{
    /// <summary>
    /// Allow creation and subscription to decoupled events
    /// </summary>
    public class EventAggregator
    {
        static EventAggregator()
        {
            _events = new Dictionary<Type, object>();
        }
        private static Dictionary<Type, object> _events;

        /// <summary>
        /// Get an event representation. The event Type signifies the sort of situation that we are interested in raising or being notified of,
        /// and the payload type represents the precise type of data payload we are interested in.
        /// </summary>
        /// <typeparam name="TEventType"></typeparam>
        /// <typeparam name="TPayload"></typeparam>
        /// <returns></returns>
        public static TEventType Get<TEventType, TPayload>() where TEventType: AggregateEvent<TPayload>
        {            
            Type t = typeof(TEventType);
            if (!_events.ContainsKey(t))
            {
                ConstructorInfo noArgs = t.GetConstructor(Type.EmptyTypes);
                TEventType evnt = (TEventType)noArgs.Invoke(null);
                _events.Add(t, evnt);
            }

            return (TEventType)_events[t];
        }

        public static TEventType Get<TEventType>() where TEventType : AggregateEvent
        {
            return GetAndStoreEvent<TEventType>();
        }

        protected static TEventType GetAndStoreEvent<TEventType>()
        {
            Type t = typeof(TEventType);
            if (!_events.ContainsKey(t))
            {
                TEventType evnt = (TEventType)Activator.CreateInstance<TEventType>();
                _events.Add(t, evnt);
            }

            return (TEventType)_events[t];
        }


    }
}
