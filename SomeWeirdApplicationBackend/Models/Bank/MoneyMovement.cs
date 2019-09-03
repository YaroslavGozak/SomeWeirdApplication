using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SomeWeirdApplicationBackend.Models.Bank
{
    public class MoneyMovement
    {
        public int Id { get; set; }

        public DateTime MovementTime { get; set; }

        public int CardInternalId { get; set; }

        public decimal Amount { get; set; }
    }
}
