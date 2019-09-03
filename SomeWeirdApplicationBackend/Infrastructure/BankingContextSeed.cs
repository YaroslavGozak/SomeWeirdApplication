using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using SomeWeirdApplicationBackend.Models.Bank;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SomeWeirdApplicationBackend.Infrastructure
{
    public class BankingContextSeed
    {
        public async Task SeedAsync(IApplicationBuilder app)
        {
            var logger = (ILogger<BankingContextSeed>)app.ApplicationServices.GetService(typeof(ILogger<BankingContextSeed>));
            var policy = CreatePolicy(logger, nameof(BankingContextSeed));

            await policy.ExecuteAsync(async () =>
            {
                var settings = (IOptions<CustomModelSettings>)app.ApplicationServices.GetService(typeof(IOptions<CustomModelSettings>));
                var useCustomizationData = settings.Value.UseCustomizationData;

                using (var serviceScope = app.ApplicationServices.CreateScope())
                {
                    var context = serviceScope.ServiceProvider.GetService<BankingContext>();
                    using (context)
                    {
                        context.Database.Migrate();
                        context.Database.EnsureCreated();
                        if (!context.Cards.Any())
                        {
                            context.Cards.AddRange(useCustomizationData
                            ? GetPreconfiguredCards()
                            : GetPreconfiguredCards());
                            await context.SaveChangesAsync();
                        }

                        if (!context.MoneyMovements.Any())
                        {
                            context.MoneyMovements.AddRange(useCustomizationData
                            ? GetPreconfiguredMoneyMovements()
                            : GetPreconfiguredMoneyMovements());
                            await context.SaveChangesAsync();
                        }
                    }
                }
            });
        }

        private IEnumerable<Card> GetPreconfiguredCards()
        {
            return new List<Card>()
            {
                new Card() { Type = CardType.Mastercard, TypeId = 1, Number = "8888 8888 8888 8888", CVV = "898", ExpirationDate = DateTime.Now.AddMonths(6), UserId = 1, CardInternalId = 1 },
                new Card() { Type = CardType.Mastercard, TypeId = 1, Number = "8888 8888 8888 8889", CVV = "798", ExpirationDate = DateTime.Now.AddMonths(6), UserId = 1, CardInternalId = 2 },
                new Card() { Type = CardType.Mastercard, TypeId = 1, Number = "8888 8888 8888 8880", CVV = "498", ExpirationDate = DateTime.Now.AddMonths(6), UserId = 2, CardInternalId = 3 },
                new Card() { Type = CardType.Visa, TypeId = 2, Number = "8888 8888 8888 9998", CVV = "878", ExpirationDate = DateTime.Now.AddMonths(6), UserId = 2, CardInternalId = 4 },
                new Card() { Type = CardType.Visa, TypeId = 2, Number = "8888 8888 8888 9898", CVV = "198", ExpirationDate = DateTime.Now.AddMonths(6), UserId = 1, CardInternalId = 5 }
            };
        }

        private IEnumerable<MoneyMovement> GetPreconfiguredMoneyMovements()
        {
            var random = new Random();
            return new List<MoneyMovement>()
            {
                new MoneyMovement() { CardInternalId = 1, MovementTime = DateTime.Now.AddDays(random.Next(-30, -1)), Amount = random.Next(10, 1000)},
                new MoneyMovement() { CardInternalId = 1, MovementTime = DateTime.Now.AddDays(random.Next(-30, -1)), Amount = random.Next(10, 1000) },
                new MoneyMovement() { CardInternalId = 1, MovementTime = DateTime.Now.AddDays(random.Next(-30, -1)), Amount = random.Next(10, 1000) },
                new MoneyMovement() { CardInternalId = 2, MovementTime = DateTime.Now.AddDays(random.Next(-30, -1)), Amount = random.Next(10, 1000) },
                new MoneyMovement() { CardInternalId = 2, MovementTime = DateTime.Now.AddDays(random.Next(-30, -1)), Amount = random.Next(10, 1000) }
            };
        }

        private AsyncRetryPolicy CreatePolicy(ILogger<BankingContextSeed> logger, string prefix, int retries = 3)
        {
            return Policy.Handle<SqlException>().
                WaitAndRetryAsync(
                    retryCount: retries,
                    sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                    onRetry: (exception, timeSpan, retry, ctx) =>
                    {
                        logger?.LogWarning(exception, "[{prefix}] Exception {ExceptionType} with message {Message} detected on attempt {retry} of {retries}", prefix, exception.GetType().Name, exception.Message, retry, retries);
                    }
                );
        }
    }
}
