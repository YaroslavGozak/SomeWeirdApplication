using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SomeWeirdApplicationBackend.Models.WebCrawler;

namespace SomeWeirdApplicationBackend.Infrastructure.EntityConfigurations
{
    public class WebSiteConfiguration: IEntityTypeConfiguration<WebSiteInfo>
    {
        public void Configure(EntityTypeBuilder<WebSiteInfo> builder)
        {
            builder.HasKey(wb => wb.Id);
            builder.Property(wb => wb.Url).IsRequired();
            builder.ForSqlServerHasIndex(wb => wb.Url);
            builder.Property(wb => wb.Domain).IsRequired();
            builder.Property(wb => wb.IsInteresting);

            //var navigation = builder.Metadata.FindNavigation(nameof(WebSiteInfo.LinkedSites));
            //navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
