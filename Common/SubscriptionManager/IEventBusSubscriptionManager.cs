using System;
using System.Collections.Generic;
using System.Text;

namespace Common.SubscriptionManager
{
    public interface IEventBusSubscriptionManager
    {
        bool IsEmpty { get; }

        void AddSubscription<T, TH>()
           where T : IntegrationEvent
           where TH : IIntegrationEventHandler<T>;

        void RemoveSubscription<T, TH>()
             where TH : IIntegrationEventHandler<T>
             where T : IntegrationEvent;

        string GetEventKey<T>();

        bool HasSubscriptionsForEvent(string eventName);

        IEnumerable<Type> GetHandlersForEvent(string eventName);

        Type GetEventTypeByName(string eventName);
    }
}
