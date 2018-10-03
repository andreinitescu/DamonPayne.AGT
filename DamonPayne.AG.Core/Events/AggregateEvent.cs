using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace DamonPayne.AG.Core.Events
{
    public class AggregateEvent
    {
        public AggregateEvent()
        {
            _subscribers = new Dictionary<Action, Func<bool>>();
        }

        internal void InvokeAll()
        {
            foreach (Action callback in _subscribers.Keys)
            {
                if (null == _subscribers[callback] || _subscribers[callback]())
                {
                    callback();
                }
            }
        }

        private Dictionary<Action, Func<bool>> _subscribers;


        public void Subscribe(Action callback)
        {
            Subscribe(callback, null);
        }

        public void Subscribe(Action callback, Func<bool> filter)
        {
            _subscribers.Add(callback, filter);
        }

        public void Raise()
        {
            InvokeAll();
        }
    }
}
