using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Xunit;
using Ambev.DeveloperEvaluation.ORM.Mapping;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Integration.Sale;

public class SaleConfigurationTests
{
    private DbContextOptions<TestDbContext> CreateInMemoryOptions()
    {
        return new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public void Sale_Configuration_Should_Set_Correct_Properties()
    {
        var options = CreateInMemoryOptions();

        using (var context = new TestDbContext(options))
        {
            var model = context.Model;
            var entityType = model.FindEntityType(typeof(Domain.Entities.Sale));
            Assert.NotNull(entityType);
            
            Assert.Equal("Sales", entityType.GetTableName());
            
            var pk = entityType.FindPrimaryKey();
            Assert.NotNull(pk);
            Assert.Contains(pk.Properties, p => p.Name == "Id");

            var saleNumberProp = entityType.FindProperty("SaleNumber");
            Assert.NotNull(saleNumberProp);
            Assert.True(saleNumberProp.IsNullable == false);

            var dateProp = entityType.FindProperty("Date");
            Assert.NotNull(dateProp);
            Assert.True(dateProp.IsNullable == false);

            var clienteIdProp = entityType.FindProperty("ClienteId");
            Assert.NotNull(clienteIdProp);
            Assert.True(clienteIdProp.IsNullable == false);

            var totalSaleAmountProp = entityType.FindProperty("TotalSaleAmount");
            Assert.NotNull(totalSaleAmountProp);
            Assert.Equal(typeof(decimal), totalSaleAmountProp.ClrType);

            var branchProp = entityType.FindProperty("Branch");
            Assert.NotNull(branchProp);
            Assert.Equal(200, branchProp.GetMaxLength());

            var statusProp = entityType.FindProperty("Status");
            Assert.NotNull(statusProp);
            Assert.Equal(typeof(SaleStatus), statusProp.ClrType);
            
            var itemsNavigation = entityType.GetNavigations();
            Assert.Contains(itemsNavigation, n => n.Name == "Items");

        }
    }

    [Fact]
    public void ItemSale_Configuration_Should_Set_Correct_Properties()
    {
        var options = CreateInMemoryOptions();

        using (var context = new TestDbContext(options))
        {
            var model = context.Model;
            var entityType = model.FindEntityType(typeof(ItemSale));
            Assert.NotNull(entityType);
            
            Assert.Equal("ItemsSale", entityType.GetTableName());

            var pk = entityType.FindPrimaryKey();
            Assert.NotNull(pk);
            Assert.Contains(pk.Properties, p => p.Name == "Id");
            
            var saleIdProp = entityType.FindProperty("SaleId");
            Assert.NotNull(saleIdProp);
            Assert.Equal(typeof(Guid), saleIdProp.ClrType);

            var descriptionProp = entityType.FindProperty("Description");
            Assert.NotNull(descriptionProp);
            Assert.Equal(200, descriptionProp.GetMaxLength());

            var quantityProp = entityType.FindProperty("Quantity");
            Assert.NotNull(quantityProp);
            Assert.False(quantityProp.IsNullable);

            var unitValueProp = entityType.FindProperty("UnitValue");
            Assert.NotNull(unitValueProp);
            Assert.Equal(typeof(decimal), unitValueProp.ClrType);

            var discountProp = entityType.FindProperty("Discount");
            Assert.NotNull(discountProp);
            Assert.Equal(typeof(decimal), discountProp.ClrType);

            var navigation = entityType.GetNavigations();
            Assert.Contains(navigation, n => n.Name == "Sale");
        }
    }

    private class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Domain.Entities.Sale> Sales { get; set; }
        public DbSet<ItemSale> ItemsSale { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new SaleConfiguration());
            modelBuilder.ApplyConfiguration(new ItemSaleConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}