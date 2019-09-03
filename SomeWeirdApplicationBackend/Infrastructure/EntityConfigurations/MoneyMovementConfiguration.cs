using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SomeWeirdApplicationBackend.Models.Bank;

namespace SomeWeirdApplicationBackend.Infrastructure.EntityConfigurations
{
    public class MoneyMovementConfiguration: IEntityTypeConfiguration<MoneyMovement>
    {
        public void Configure(EntityTypeBuilder<MoneyMovement> builder)
        {
            builder.HasKey(cm => cm.Id);

            builder.Property(cm => cm.CardInternalId)
               .IsRequired();

            builder.Property(cm => cm.Amount)
                .IsRequired()
                .HasColumnType("money");

            builder.Property(cm => cm.MovementTime)
                .IsRequired()
                .HasMaxLength(100);
        }
    }
}
