using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Handling.API.Events;

namespace Handling.API.EventHandlers
{
    public class ProductPriceChangedEventHandler : IIntegrationEventHandler<ProductPriceChangedEvent>
    {
        public Task Handle(ProductPriceChangedEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}
