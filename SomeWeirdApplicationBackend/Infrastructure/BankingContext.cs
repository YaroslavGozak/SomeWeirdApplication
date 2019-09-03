using Microsoft.EntityFrameworkCore;
using SomeWeirdApplicationBackend.Infrastructure.EntityConfigurations;
using SomeWeirdApplicationBackend.Models.Bank;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SomeWeirdApplicationBackend.Infrastructure
{
    public class BankingContext: DbContext
    {
        public BankingContext(DbContextOptions<BankingContext> options) : base(options)
        {
        }
        public DbSet<Card> Cards { get; set; }
        public DbSet<MoneyMovement> MoneyMovements { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new CardConfiguration());
            builder.ApplyConfiguration(new MoneyMovementConfiguration());
        }
    }
}
