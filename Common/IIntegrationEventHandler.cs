using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public interface IIntegrationEventHandler<T> where T: IntegrationEvent
    {
        Task Handle(T @event);
    }
}
