using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Handling.API.Events
{
    public class ProductPriceChangedEvent: IntegrationEvent
    {
        public string ProductId { get; private set; }

        public decimal OldPrice { get; private set; }

        public decimal NewPrice { get; private set; }

        public ProductPriceChangedEvent(string productId, decimal oldPrice, decimal newPrice)
        {
            ProductId = productId;
            OldPrice = oldPrice;
            NewPrice = newPrice;
        }
    }
}
