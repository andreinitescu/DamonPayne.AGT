using System.Collections.Generic;
using DamonPayne.AG.Core.DataTypes;
using DamonPayne.AG.Core.Events;
using DamonPayne.AG.Core;

namespace DamonPayne.HTLayout.Services
{
    public class MemoryLogger : ILogService
    {
        public MemoryLogger()
        {
            _messages = new List<LogMessage>();
            _logEvent = EventAggregator.Get<MessageArrivedEvent<LogMessage>, LogMessage>();            
        }

        private List<LogMessage> _messages;
        
        private MessageArrivedEvent<LogMessage> _logEvent;

        public void Log(LogMessage m)
        {
            _messages.Add(m);
            _logEvent.Raise(m);
        }

    }
}
