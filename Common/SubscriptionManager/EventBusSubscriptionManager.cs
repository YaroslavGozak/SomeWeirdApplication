using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.SubscriptionManager
{
    class EventBusSubscriptionManager: IEventBusSubscriptionManager
    {
        private readonly Dictionary<string, List<Type>> _handlers;
        private readonly List<Type> _events;

        public EventBusSubscriptionManager()
        {
            _handlers = new Dictionary<string, List<Type>>();
            _events = new List<Type>();
        }

        public bool IsEmpty => !_handlers.Values.Any();

        public void AddSubscription<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = GetEventKey<T>();
            var keyExists = _handlers.TryGetValue(eventName, out var handlers);
            if (keyExists)
            {
                if (handlers.Contains(typeof(TH)))
                    throw new ArgumentException($"Handler {typeof(TH).Name} already bounded to {eventName}");
                else
                    handlers.Add(typeof(TH));
            }
            else
            {
                _handlers[eventName] = new List<Type> { typeof(TH) };
                _events.Add(typeof(T));
            }
        }

        public string GetEventKey<T>()
        {
            return typeof(T).Name;
        }

        public Type GetEventTypeByName(string eventName)
        {
            return _events.FirstOrDefault(e => e.Name == eventName);
        }

        public IEnumerable<Type> GetHandlersForEvent(string eventName)
        {
            return _handlers[eventName];
        }

        public bool HasSubscriptionsForEvent(string eventName)
        {
            return _handlers.ContainsKey(eventName);
        }

        public void RemoveSubscription<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = GetEventKey<T>();
            var keyExists = _handlers.TryGetValue(eventName, out var handlers);
            if (!keyExists)
                throw new ArgumentException($"No event {eventName} found");
            if (!handlers.Contains(typeof(TH)))
                throw new ArgumentException($"Handler {typeof(TH)} is not bound to {eventName}");
            handlers.Remove(typeof(TH));

            if (!handlers.Any())
            {
                _handlers.Remove(eventName);
                _events.Remove(typeof(T));
            } 
        }
    }
}
