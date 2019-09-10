using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SomeWeirdApplicationBackend.Models;
using Microsoft.Extensions.Configuration;

namespace SomeWeirdApplicationBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        private readonly CustomModelSettings _settings;
        private readonly IConfiguration _configuration;

        public DataController(IOptionsSnapshot<CustomModelSettings> settings, IConfiguration configuration
            )
        {
            //_catalogIntegrationEventService = catalogIntegrationEventService ?? throw new ArgumentNullException(nameof(catalogIntegrationEventService));
            _settings = settings.Value;

            _configuration = configuration;
        }

        // GET api/data
        [HttpGet]
        public IEnumerable<PointData> Get()
        {
            var randomizer = new Random();

            var pointCollection1 = new PointData
            {
                Name = "Beverly",
                TurboThreshold = 5000000
            };
            var pointCollection2 = new PointData
            {
                Name = "Hilton",
                TurboThreshold = 5000000
            };
            var points1 = new List<object[]>();
            var points2 = new List<object[]>();

            for (var i = 0; i < 150; i++)
            {
                points1.Add(new object[]
                {
                    DateTime.Now.AddHours(randomizer.Next(1, 24)).ToString("yyyy-MM-dd hh:mm:ss"),
                    randomizer.Next(2, 70)
                });

                points2.Add(new object[]
                {
                    DateTime.Now.AddDays(-1).AddHours(randomizer.Next(1, 24)).ToString("yyyy-MM-dd hh:mm:ss"),
                    randomizer.Next(2, 70)
                });
            }

            pointCollection1.Data = points1;
            pointCollection2.Data = points2;

            var data = new List<PointData> { pointCollection1, pointCollection2 };
            return data;
        }
    }
}
