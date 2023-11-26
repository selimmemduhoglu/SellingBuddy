using CatalogService.Api.Core.Domain;
using CatalogService.Api.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogService.Api.Infrastructure.EntityConfigurations
{
    public class CatalogItemEntityTypeConfiguration : IEntityTypeConfiguration<CatalogItem>
    {
        public void Configure(EntityTypeBuilder<CatalogItem> builder)
        {
            builder.ToTable("Catalog", CatalogContext.DEFAULT_SCHEMA);

            builder.Property(ci => ci.Id)
                .UseHiLo("catalog_hilo") //otomatik artan olması için koyduk.
                .IsRequired();

            builder.Property(ci => ci.Name)
               .IsRequired(true)
               .HasMaxLength(50);

            builder.Property(ci => ci.Price)
               .IsRequired(true);

            builder.Property(ci => ci.PictureFileName)
               .IsRequired(false);

            builder.Ignore(ci => ci.PictureUri); //bunun amacı veri tabanı tablosu oluşturulurken burası sayesinde burada ki PictureUri yi göz ardı etmek amacıyla yazılmış. Yani db de kolonu oluşmayacak ama kod tarafında kullanılacak.

            builder.HasOne(ci => ci.CatalogBrand)
                .WithMany()
                .HasForeignKey(ci => ci.CatalogBrandId);

            builder.HasOne(ci => ci.CatalogType)
                .WithMany()
                .HasForeignKey(ci => ci.CatalogTypeId);

        }
    }
}
