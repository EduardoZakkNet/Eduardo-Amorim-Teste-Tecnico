namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

/// <summary>
/// Represents a request to update a new Item Sale in the system.
/// </summary>
public class UpdateItemSaleRequest
{
    /// <summary>
    /// Gets the Item Sale Id.
    /// </summary>
    public Guid Id { get; set; }
    
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
}