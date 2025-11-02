using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Entities.Interfaces;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents a Item Sale in the system.
/// This entity follows domain-driven design principles and includes business rules validation.
/// </summary>
public class ItemSale : BaseEntity, IItemSale
{
    /// <summary>
    /// Gets the unique identifier of the Item Sale.
    /// </summary>
    /// <returns>The Itens Sale ID as a string.</returns>
    string IItemSale.Id => Id.ToString();
    
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
    /// Gets the Discount Value of Product.
    /// </summary>
    public decimal Discount { get; set; } = 0m;
    
    /// <summary>
    /// Gets the Total Itens Value of Product.
    /// </summary>
    public decimal TotalItem { get; set; } = 0m;
    
    /// <summary>
    /// Gets the Sale Id.
    /// </summary>
    public Guid SaleId { get; set; } = Guid.Empty;
    
    /// <summary>
    /// Gets the Sale Object.
    /// </summary>
    public Sale? Sale { get; set; }
    
    /// <summary>
    /// Gets the date and time when the item sale was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets the date and time of the last update to the item's sale information.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
    
    /// <summary>
    /// Initializes a new instance of the Item Sale class.
    /// </summary>
    public ItemSale()
    {
        CreatedAt = DateTime.UtcNow;
    }
}