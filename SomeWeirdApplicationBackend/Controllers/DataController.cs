using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SomeWeirdApplicationBackend.Infrastructure;
using SomeWeirdApplicationBackend.Models;
using SomeWeirdApplicationBackend.ViewModels;
using Dapper;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace SomeWeirdApplicationBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        private readonly BankingContext _context;
        private readonly CustomModelSettings _settings;
        private readonly IConfiguration _configuration;

        public DataController(BankingContext context, IOptionsSnapshot<CustomModelSettings> settings, IConfiguration configuration
            )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            //_catalogIntegrationEventService = catalogIntegrationEventService ?? throw new ArgumentNullException(nameof(catalogIntegrationEventService));
            _settings = settings.Value;

            _configuration = configuration;

            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
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

        // GET api/v1/[controller]/items[?pageSize=3&pageIndex=10]
        [HttpGet]
        [Route("items")]
        [ProducesResponseType(typeof(PaginatedItemsViewModel<CustomModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(IEnumerable<CustomModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ItemsAsync([FromQuery]int pageSize = 10, [FromQuery]int pageIndex = 0, string ids = null)
        {
            if (!string.IsNullOrEmpty(ids))
            {
                var items = await GetItemsByIdsAsync(ids);

                if (!items.Any())
                {
                    return BadRequest("ids value invalid. Must be comma-separated list of numbers");
                }

                return Ok(items);
            }

            //var totalItems = await _modelContext.CustomModelItems
            //    .LongCountAsync();

            //var itemsOnPage = await _modelContext.CustomModelItems
            //    .OrderBy(c => c.Value)
            //    .Skip(pageSize * pageIndex)
            //    .Take(pageSize)
            //    .ToListAsync();

            var model = new PaginatedItemsViewModel<CustomModel>(pageIndex, pageSize, 0, null);

            return Ok(model);
        }

        [HttpGet]
        [Route("bases")]
        [ProducesResponseType(typeof(PaginatedItemsViewModel<CustomModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(IEnumerable<CustomModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> DatabasesAsync([FromQuery]int pageSize = 10, [FromQuery]int pageIndex = 0, string ids = null)
        {
            //if (!string.IsNullOrEmpty(ids))
            //{
            //    var items = await GetItemsByIdsAsync(ids);

            //    if (!items.Any())
            //    {
            //        return BadRequest("ids value invalid. Must be comma-separated list of numbers");
            //    }

            //    return Ok(items);
            //}

            //using (var context = new SqlConnection(_configuration["ConnectionString"]))
            //{
            //    try
            //    {
            //        context.Open();
            //        var databases = context.Query<string>(@"SELECT Name FROM sys.Databases", commandTimeout: 60);

            //        return Ok(databases);
            //    }
            //    catch(Exception ex)
            //    {
            //        return BadRequest(ex.Message);
            //    }
            //}; 

            var cards = await _context.Cards.ToListAsync();
            return Ok(cards);
        }

        private async Task<List<CustomModel>> GetItemsByIdsAsync(string ids)
        {
            var numIds = ids.Split(',').Select(id => (Ok: int.TryParse(id, out int x), Value: x));

            if (!numIds.All(nid => nid.Ok))
            {
                return new List<CustomModel>();
            }

            var idsToSelect = numIds
                .Select(id => id.Value);

            //var items = await _modelContext.CustomModelItems.Where(cm => idsToSelect.Contains(cm.Id)).ToListAsync();

            return null;
        }
    }
}
