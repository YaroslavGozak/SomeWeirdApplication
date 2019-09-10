using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SomeWeirdApplicationBackend.Models.Bank;
using SomeWeirdApplicationBackend.Models.WebCrawler;

namespace SomeWeirdApplicationBackend.Infrastructure.EntityConfigurations
{
    public class WebSiteConfiguration: IEntityTypeConfiguration<WebSiteInfo>
    {
        public void Configure(EntityTypeBuilder<WebSiteInfo> builder)
        {
            builder.HasKey(wb => wb.Id);

            builder.Property(wb => wb.Url)
                .IsRequired();

            var navigation = builder.Metadata.FindNavigation(nameof(WebSiteInfo.LinkedSites));
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
