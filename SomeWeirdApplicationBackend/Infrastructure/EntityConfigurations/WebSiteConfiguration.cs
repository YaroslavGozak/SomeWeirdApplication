using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SomeWeirdApplicationBackend.Models.Bank;
using SomeWeirdApplicationBackend.Models.WebCrawler;

namespace SomeWeirdApplicationBackend.Infrastructure.EntityConfigurations
{
    public class WebSiteConfiguration: IEntityTypeConfiguration<WebSiteStatistics>
    {
        public void Configure(EntityTypeBuilder<WebSiteStatistics> builder)
        {
            builder.HasKey(wb => wb.InternalId);

            builder.Property(wb => wb.Url)
                .IsRequired();
        }
    }
}
