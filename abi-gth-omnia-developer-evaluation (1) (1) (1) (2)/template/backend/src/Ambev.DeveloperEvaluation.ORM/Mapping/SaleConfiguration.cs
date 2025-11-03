using System.Diagnostics.CodeAnalysis;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

[ExcludeFromCodeCoverage]
public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("Sales");

        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).HasColumnType("uuid").HasDefaultValueSql("gen_random_uuid()");
        
        builder.Property(u => u.SaleNumber)
            .IsRequired();

        builder.Property(u => u.Date)
            .IsRequired();

        builder.Property(u => u.ClienteId)
            .IsRequired();

        builder.Property(u => u.TotalSaleAmount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(u => u.Branch)
            .HasMaxLength(200)
            .IsRequired();
        
        builder.Property(u => u.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();
        
        builder.HasMany(u => u.Items)
            .WithOne()
            .HasForeignKey("SaleId")
            .OnDelete(DeleteBehavior.Cascade);

        // Lazy Loading
        builder.Navigation(u => u.Items).UsePropertyAccessMode(PropertyAccessMode.Property);
        
        builder.Property(u => u.Status)
            .HasConversion<string>()
            .HasMaxLength(20);
    }
}