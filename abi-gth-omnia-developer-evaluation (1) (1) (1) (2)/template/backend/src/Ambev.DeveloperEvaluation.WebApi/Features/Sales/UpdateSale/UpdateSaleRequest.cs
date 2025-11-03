using System.Diagnostics.CodeAnalysis;
using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

/// <summary>
/// Represents a request to update a new Sale in the system.
/// </summary>
[ExcludeFromCodeCoverage]
public class UpdateSaleRequest
{
    /// <summary>
    /// Gets the Sale Id.
    /// </summary>
    public Guid Id { get; set; }
    
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
    /// Gets the Status of Sale.
    /// </summary>
    public SaleStatus Status { get; set; }
    
    /// <summary>
    /// Gets or sets the Branch. Must be date valid.
    /// </summary>
    public string Branch { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the initial Items of the Sale.
    /// </summary>
    public IEnumerable<UpdateItemSaleRequest>? Items { get; set; }
}