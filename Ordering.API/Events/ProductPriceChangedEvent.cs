using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.API.Events
{
    public class ProductPriceChangedEvent: IntegrationEvent
    {
        public string ProductId { get; private set; }

        public decimal OldPrice { get; set; }

        public decimal NewPrice { get; set; }
    }
}
