using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Domain.Enums;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

/// <summary>
/// Command for update a new sale.
/// </summary>
/// <remarks>
/// This command is used to capture the required data for creating a sale, 
/// including sale date, SaleDate, total sales amount, Branch, and Itens List. 
/// It implements <see cref="IRequest{TResponse}"/> to initiate the request 
/// that returns a <see cref="UpdateSaleResult"/>.
/// 
/// populated and follow the required rules.
/// </remarks>
public class UpdateSaleCommand: IRequest<UpdateSaleResult>
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
    public DateTime? Date { get; set; }
    
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
    public IEnumerable<UpdateItemSaleCommand>? Items { get; set; }
    
    public ValidationResultDetail Validate()
    {
        var validator = new UpdateSaleCommandValidador();
        var result = validator.Validate(this);
        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(o => (ValidationErrorDetail)o)
        };
    }
}