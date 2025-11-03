using System.Diagnostics.CodeAnalysis;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.DeleteSale;

/// <summary>
/// Request model for deleting a Sale
/// </summary>
[ExcludeFromCodeCoverage]
public class DeleteSaleRequest
{
    /// <summary>
    /// The unique identifier of the Sale to delete
    /// </summary>
    public Guid Id { get; set; }
}