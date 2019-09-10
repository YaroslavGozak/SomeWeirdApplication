using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SomeWeirdApplicationBackend.Models.WebCrawler
{
    public class WebSiteInfo
    {
        public string Url { get; set; }

        public string Domain { get; set; }

        public int Count { get; set; }

        public ICollection<WebSiteInfo> LinkedSites { get; set; }

        public int Id { get; set; }
    }
}
