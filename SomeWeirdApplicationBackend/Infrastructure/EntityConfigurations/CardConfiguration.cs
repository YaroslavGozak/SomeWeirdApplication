using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SomeWeirdApplicationBackend.Models.Bank;

namespace SomeWeirdApplicationBackend.Infrastructure.EntityConfigurations
{
    public class CardConfiguration: IEntityTypeConfiguration<Card>
    {
        public void Configure(EntityTypeBuilder<Card> builder)
        {
            builder.HasKey(cm => cm.CardInternalId);

            builder.Property(cm => cm.CardInternalId)
               .ForSqlServerUseSequenceHiLo("card_hilo")
               .IsRequired();

            builder.Property(cm => cm.Number)
                .IsRequired();

            builder.Property(cm => cm.ExpirationDate)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(cm => cm.CVV)
                .IsRequired();

            builder.Property(cm => cm.UserId)
                .IsRequired();

            builder.Property(cm => cm.TypeId)
                .IsRequired();
        }
    }
}
