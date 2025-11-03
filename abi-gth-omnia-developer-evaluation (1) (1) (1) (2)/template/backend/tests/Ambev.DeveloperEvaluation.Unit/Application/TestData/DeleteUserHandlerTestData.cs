using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain;

/// <summary>
/// Provides methods for generating test data using the Bogus library.
/// This class centralizes all test data generation to ensure consistency
/// across test cases and provide both valid and invalid data scenarios.
/// </summary>
public static class DeleteUserHandlerTestData
{
    /// <summary>
    /// Configures the Faker to generate valid User entities.
    /// The generated Sale will have valid:
    /// - Id (Sale number valid)
    /// </summary>
    private static readonly Faker<DeleteSaleCommand> deleteSaleHandlerFaker = new Faker<DeleteSaleCommand>()
        .RuleFor(c => c.Id, f => f.Random.Guid());

    /// <summary>
    /// Generates a valid Sale entity with randomized data.
    /// The generated user will have all properties populated with valid values
    /// that meet the system's validation requirements.
    /// </summary>
    /// <returns>A valid Sale entity with randomly generated data.</returns>
    public static DeleteSaleCommand GenerateValidCommand()
    {
        return deleteSaleHandlerFaker.Generate();
    }
}