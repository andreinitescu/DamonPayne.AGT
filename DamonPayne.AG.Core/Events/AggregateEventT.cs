using System;
using System.Collections.Generic;

namespace DamonPayne.AG.Core.Events
{
    
    public class AggregateEvent<T>
    {
        public AggregateEvent()
        {
            _subscribers = new Dictionary<Action<T>, Func<T, bool>>();
        }

        internal void InvokeAll(T arg)
        {
            foreach (Action<T> callback in _subscribers.Keys)
            {
                if (null == _subscribers[callback] || _subscribers[callback](arg))
                {
                    callback(arg);
                }
            }
        }

        private Dictionary<Action<T>, Func<T, bool>> _subscribers;


        public void Subscribe(Action<T> callback)
        {
            Subscribe(callback, null);
        }

        public void Subscribe(Action<T> callback, Func<T, bool> filter)
        {
            _subscribers.Add(callback, filter);
        }

        public void Raise(T payload)
        {
            InvokeAll(payload);
        }

        public static Action<T> operator + (AggregateEvent<T> agEvent, Action<T> callback) 
        {
            agEvent.Subscribe(callback);
            return callback;
        }

    }
}
