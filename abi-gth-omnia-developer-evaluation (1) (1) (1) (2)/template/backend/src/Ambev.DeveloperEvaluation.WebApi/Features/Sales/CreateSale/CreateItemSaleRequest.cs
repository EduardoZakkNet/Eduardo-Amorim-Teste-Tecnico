using System.Diagnostics.CodeAnalysis;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

/// <summary>
/// Represents a request to create a new Item Sale in the system.
/// </summary>
[ExcludeFromCodeCoverage]
public class CreateItemSaleRequest
{
    /// <summary>
    /// Gets the Description of Product.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets the Quantity of Product.
    /// </summary>
    public int Quantity { get; set; } = 0;
    
    /// <summary>
    /// Gets the Unit Value of Product.
    /// </summary>
    public decimal UnitValue { get; set; } = 0m;
    
    /// <summary>
    /// Gets the date and time when the item sale was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Initializes a new instance of the Item Sale class.
    /// </summary>
    public CreateItemSaleRequest()
    {
        CreatedAt = DateTime.UtcNow;
    }
}