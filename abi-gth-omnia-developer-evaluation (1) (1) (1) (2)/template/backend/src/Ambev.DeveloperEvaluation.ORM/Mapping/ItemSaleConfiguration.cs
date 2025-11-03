using System.Diagnostics.CodeAnalysis;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;
[ExcludeFromCodeCoverage]
public class ItemSaleConfiguration : IEntityTypeConfiguration<ItemSale>
{
    public void Configure(EntityTypeBuilder<ItemSale> builder)
    {
        builder.ToTable("ItemsSale");

        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).HasColumnType("uuid").HasDefaultValueSql("gen_random_uuid()");
        
        builder.Property(i => i.SaleId)
            .HasColumnType("uuid")
            .IsRequired();
        
        builder.HasOne(i => i.Sale)
            .WithMany(s => s.Items)
            .HasForeignKey(i => i.SaleId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Property(i => i.Description)
            .IsRequired()
            .HasMaxLength(200); 

        builder.Property(i => i.Quantity)
            .IsRequired();

        builder.Property(i => i.UnitValue)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(i => i.Discount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
    }
}