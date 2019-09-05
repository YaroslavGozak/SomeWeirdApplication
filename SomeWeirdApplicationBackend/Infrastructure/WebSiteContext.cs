using Microsoft.EntityFrameworkCore;
using SomeWeirdApplicationBackend.Infrastructure.EntityConfigurations;
using SomeWeirdApplicationBackend.Models.WebCrawler;

namespace SomeWeirdApplicationBackend.Infrastructure
{
    public class WebSiteContext: DbContext
    {
        public WebSiteContext(DbContextOptions<WebSiteContext> options) : base(options)
        {
        }
        public DbSet<WebSiteStatistics> WebSites { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new WebSiteConfiguration());
        }
    }
}
