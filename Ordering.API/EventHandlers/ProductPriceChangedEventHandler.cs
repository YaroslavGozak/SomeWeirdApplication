using Common;
using Ordering.API.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.API.EventHandlers
{
    public class ProductPriceChangedEventHandler : IIntegrationEventHandler<ProductPriceChangedEvent>
    {
        public Task Handle(ProductPriceChangedEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}
