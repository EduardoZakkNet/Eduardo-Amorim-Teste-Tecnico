using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain;

/// <summary>
/// Provides methods for generating test data using the Bogus library.
/// This class centralizes all test data generation to ensure consistency
/// across test cases and provide both valid and invalid data scenarios.
/// </summary>
public class UpdateSaleHandlerTestData
{
        /// <summary>
    /// Configures the Faker to generate valid User entities.
    /// The generated Sale will have valid:
    /// - SaleNumber (Sale number valid)
    /// - Date (Date Sale requirements)
    /// - ClienteId (valid format Guid)
    /// - Branch (Branch namme)
    /// - Items (Active or Suspended)
    /// </summary>
    private static readonly Faker<UpdateSaleCommand> updateSaleHandlerFaker = new Faker<UpdateSaleCommand>()
        .RuleFor(c => c.Id, f => f.Random.Guid())
        .RuleFor(c => c.SaleNumber, f => f.Random.Int(1, 10000))
        .RuleFor(c => c.Date, f => f.Date.Past(2)) 
        .RuleFor(c => c.ClienteId, f => Guid.NewGuid())
        .RuleFor(c => c.Branch, f => f.Address.City())
        .RuleFor(c => c.Items, f => GenerateItemsCommand(f.Random.Int(1, 5)));
    
    /// <summary>
    /// Generates a Random Lisf of Item that be use in a Sale.
    /// that meet the system's validation requirements.
    /// </summary>
    ///<see cref="f"/> faker Item will gerated
    /// <returns>A new list of Item Sale</returns>
    private static IEnumerable<UpdateItemSaleCommand> GenerateItemsCommand(int count)
    {
        var faker = new Faker<UpdateItemSaleCommand>()
            .RuleFor(p => p.Description, f => f.Commerce.ProductName())
            .RuleFor(p => p.Quantity, f => f.Random.Number(1, 5))
            .RuleFor(p => p.UnitValue, f => f.Random.Decimal(100000000, 999999999));
        return faker.Generate(count);
    }
    
    /// <summary>
    /// Generates a Random Lisf of Item that be use in a Sale.
    /// that meet the system's validation requirements.
    /// </summary>
    /// <returns>A new Sale</returns>
    private static readonly Faker<Sale> saleFaker = new Faker<Sale>()
        .RuleFor(s => s.SaleNumber, f => f.Random.Int(1, 10000))
        .RuleFor(s => s.Date, f => f.Date.Past(1))
        .RuleFor(s => s.ClienteId, f => f.Random.Guid())
        .RuleFor(s => s.TotalSaleAmount, f => f.Random.Decimal(10, 10000))
        .RuleFor(s => s.Branch, f => f.Company.CompanyName())
        .RuleFor(s => s.Status, f => f.PickRandom(SaleStatus.Cancelled, SaleStatus.Active, SaleStatus.Unknown))
        .RuleFor(s => s.Items, f => GenerateItems(f.Random.Int(1, 5)));

    /// <summary>
    /// Generates a Random Lisf of Item that be use in a Sale.
    /// that meet the system's validation requirements.
    /// </summary>
    ///<see cref="count"/> number of Item will generate
    /// <returns>A new list of Item Sale</returns>
    private static List<ItemSale> GenerateItems(int count)
    {
        var items = new List<ItemSale>();
        for (int i = 0; i < count; i++)
        {
            items.Add(new ItemSale
            {
                Quantity = new Random().Next(1, 20),
                UnitValue = new decimal(new Random().Next(1, 1000))
            });
        }
        return items;
    }
    
    /// <summary>
    /// Generates a valid Sale entity with randomized data.
    /// The generated user will have all properties populated with valid values
    /// that meet the system's validation requirements.
    /// </summary>
    /// <returns>A valid Sale entity with randomly generated data.</returns>
    public static UpdateSaleCommand GenerateValidCommand()
    {
        return updateSaleHandlerFaker.Generate();
    }
}