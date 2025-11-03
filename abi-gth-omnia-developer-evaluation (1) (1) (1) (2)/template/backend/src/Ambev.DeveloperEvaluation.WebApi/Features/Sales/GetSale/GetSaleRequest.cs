using System.Diagnostics.CodeAnalysis;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;

/// <summary>
/// Request model for getting a Sale by ID
/// </summary>
[ExcludeFromCodeCoverage]
public class GetSaleRequest
{
    /// <summary>
    /// The unique identifier of the Sale to retrieve
    /// </summary>
    public Guid Id { get; set; }
}