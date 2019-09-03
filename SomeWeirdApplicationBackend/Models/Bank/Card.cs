using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SomeWeirdApplicationBackend.Models.Bank
{
    public class Card
    {
        public int UserId { get; set; }

        public int CardInternalId { get; set; }

        public int TypeId { get; set; }

        public string Number { get; set; }

        public string CVV { get; set; }

        public DateTime ExpirationDate { get; set; }

        public string HolderName { get; set; }

        public CardType Type { get; set; }
    }
}
