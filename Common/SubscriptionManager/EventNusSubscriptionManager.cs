using System;
using System.Collections.Generic;
using System.Text;

namespace Common.SubscriptionManager
{
    class EventNusSubscriptionManager
    {
        private readonly Dictionary<string, List<IIntegrationEventHandler>> _handlers;
        private readonly List<Type> _eventTypes;

        public EventNusSubscriptionManager()
        {
            _handlers = new Dictionary<string, List<SubscriptionInfo>>();
            _eventTypes = new List<Type>();
        }
    }
}
