using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using SomeWeirdApplicationBackend.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SomeWeirdApplicationBackend.Infrastructure.EntityConfigurations
{
    public class ModelContextSeed
    {
        public async Task SeedAsync(IApplicationBuilder app)
        {
            var logger = (ILogger<ModelContextSeed>)app.ApplicationServices.GetService(typeof(ILogger<ModelContextSeed>));
            var policy = CreatePolicy(logger, nameof(ModelContextSeed));

            await policy.ExecuteAsync(async () =>
            {
                var settings = (IOptions<CustomModelSettings>)app.ApplicationServices.GetService(typeof(IOptions<CustomModelSettings>));
                var useCustomizationData = settings.Value.UseCustomizationData;

                using (var serviceScope = app.ApplicationServices.CreateScope())
                {
                    var context = serviceScope.ServiceProvider.GetService<ModelContext>();
                    using (context)
                    {
                        context.Database.Migrate();
                        if (!context.CustomModelItems.Any())
                        {
                            context.CustomModelItems.AddRange(useCustomizationData
                            ? null
                            : GetPreconfiguredCustomModels());
                            await context.SaveChangesAsync();
                        }
                    }
                }   
            });
        }

        private IEnumerable<CustomModel> GetCustomModelsFromFile(string contentRootPath, ILogger<ModelContextSeed> logger)
        {
            string csvFileCatalogBrands = Path.Combine(contentRootPath, "Setup", "CustomModels.csv");

            if (!File.Exists(csvFileCatalogBrands))
            {
                return GetPreconfiguredCustomModels();
            }

            string[] csvheaders;
            try
            {
                string[] requiredHeaders = { "custommodel" };
                csvheaders = GetHeaders(csvFileCatalogBrands, requiredHeaders);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message);
                return GetPreconfiguredCustomModels();
            }

            return File.ReadAllLines(csvFileCatalogBrands)
                                        .Skip(1) // skip header row
                                        .Select(x => CreateCustomModel(x))
                                        .Where(x => x != null);
        }

        private CustomModel CreateCustomModel(string model)
        {
            model = model.Trim('"').Trim();

            if (String.IsNullOrEmpty(model))
            {
                throw new Exception("model is empty");
            }

            return new CustomModel
            {
                Value = model,
            };
        }

        private IEnumerable<CustomModel> GetPreconfiguredCustomModels()
        {
            return new List<CustomModel>()
            {
                new CustomModel() { Value = "Azure"},
                new CustomModel() { Value = ".NET" },
                new CustomModel() { Value = "Visual Studio" },
                new CustomModel() { Value = "SQL Server" },
                new CustomModel() { Value = "Other" }
            };
        }

        private AsyncRetryPolicy CreatePolicy(ILogger<ModelContextSeed> logger, string prefix, int retries = 3)
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

        private string[] GetHeaders(string csvfile, string[] requiredHeaders, string[] optionalHeaders = null)
        {
            string[] csvheaders = File.ReadLines(csvfile).First().ToLowerInvariant().Split(',');

            if (csvheaders.Count() < requiredHeaders.Count())
            {
                throw new Exception($"requiredHeader count '{ requiredHeaders.Count()}' is bigger then csv header count '{csvheaders.Count()}' ");
            }

            if (optionalHeaders != null)
            {
                if (csvheaders.Count() > (requiredHeaders.Count() + optionalHeaders.Count()))
                {
                    throw new Exception($"csv header count '{csvheaders.Count()}'  is larger then required '{requiredHeaders.Count()}' and optional '{optionalHeaders.Count()}' headers count");
                }
            }

            foreach (var requiredHeader in requiredHeaders)
            {
                if (!csvheaders.Contains(requiredHeader))
                {
                    throw new Exception($"does not contain required header '{requiredHeader}'");
                }
            }

            return csvheaders;
        }

    }
}
