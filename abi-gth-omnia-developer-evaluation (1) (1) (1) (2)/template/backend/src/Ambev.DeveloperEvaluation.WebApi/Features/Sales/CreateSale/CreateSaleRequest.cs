
using System.Diagnostics.CodeAnalysis;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

/// <summary>
/// Represents a request to create a new Sale in the system.
/// </summary>
[ExcludeFromCodeCoverage]
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
    /// Gets or sets the Branch. Must be date valid.
    /// </summary>
    public string Branch { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the initial Items of the Sale.
    /// </summary>
    public IEnumerable<CreateItemSaleRequest>? Items { get; set; }
}