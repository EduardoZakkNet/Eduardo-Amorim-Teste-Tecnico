using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

/// <summary>
/// Response model for GetSale operation
/// </summary>
public class GetSaleResult
{
    public GetSaleResult(List<ItemSale> items)
    {
        Items = items;
    }

    /// <summary>
    /// The unique identifier of the created Sale
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// The Sale Number.
    /// </summary>
    public int SaleNumber { get; set; } = 0;
    
    /// <summary>
    /// The Date of Sale
    /// </summary>
    public DateTime Date { get; set; }
    
    /// <summary>
    /// The Client Id
    /// </summary>
    public Guid ClienteId { get; set; } = Guid.Empty;
    
    /// <summary>
    /// The Total Sales Amount.
    /// </summary>
    public decimal TotalSaleAmount { get; set; } = 0m;
    
    /// <summary>
    /// The Branch
    /// </summary>
    public string Branch { get; set; } = string.Empty;
    
    /// <summary>
    /// The Status of Sale
    /// </summary>
    public SaleStatus Status { get; set; }
    
    /// <summary>
    /// The items of the Sale.
    /// </summary>
    public List<ItemSale> Items { get; set; }
}