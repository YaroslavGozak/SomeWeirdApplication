using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SomeWeirdApplicationBackend.Models.WebCrawler
{
    public class WebSiteStatistics
    {
        public string Url { get; set; }

        public int Count { get; set; }

        public IEnumerable<WebSiteStatistics> LinkedSites { get; set; }

        public int InternalId { get; set; }
    }
}
