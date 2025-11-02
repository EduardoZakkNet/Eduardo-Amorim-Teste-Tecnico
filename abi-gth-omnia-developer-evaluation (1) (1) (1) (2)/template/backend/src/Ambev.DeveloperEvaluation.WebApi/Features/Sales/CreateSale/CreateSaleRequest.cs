using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

/// <summary>
/// Represents a request to create a new Sale in the system.
/// </summary>
public class CreateSaleRequest
{
    /// <summary>
    /// Gets or sets the Sale Number. Must be date valid.
    /// </summary>
    public int SaleNumber { get; set; } = 0;
    
    /// <summary>
    /// Gets or sets the Date of Sale. Must be date valid.
    /// </summary>
    public DateTime Date { get; set; }
    
    /// <summary>
    /// Gets or sets the Client Id. Must be date valid.
    /// </summary>
    public Guid ClienteId { get; set; } = Guid.Empty;
    
    /// <summary>
    /// Gets or sets the Total Sales Amount. Must be valid and greater than zero.
    /// </summary>
    public decimal TotalSaleAmount { get; set; } = 0m;
    
    /// <summary>
    /// Gets or sets the Branch. Must be date valid.
    /// </summary>
    public string Branch { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the initial status of the Sale.
    /// </summary>
    public SaleStatus Status { get; set; }
    
    /// <summary>
    /// Gets or sets the initial Items of the Sale.
    /// </summary>
    public IEnumerable<ItemSale>? Items { get; set; }
}