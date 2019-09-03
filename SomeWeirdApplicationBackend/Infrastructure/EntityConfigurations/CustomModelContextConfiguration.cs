using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SomeWeirdApplicationBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SomeWeirdApplicationBackend.Infrastructure
{
    public class CustomModelContextConfiguration : IEntityTypeConfiguration<CustomModel>
    {
        public void Configure(EntityTypeBuilder<CustomModel> builder)
        {
            builder.ToTable("CustomModels");

            builder.HasKey(cm => cm.Id);

            builder.Property(cm => cm.Id)
               .ForSqlServerUseSequenceHiLo("custom_model_hilo")
               .IsRequired();

            builder.Property(cm => cm.CreatedBy)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(cm => cm.CreatedDate)
                .IsRequired();
        }
    }
}
