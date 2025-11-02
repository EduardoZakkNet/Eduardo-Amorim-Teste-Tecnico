using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Entities.Interfaces;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Validation;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class Sale : BaseEntity, ISale
{
    /// <summary>
    /// Gets the unique identifier of the Sale.
    /// </summary>
    /// <returns>The Itens Sale ID as a string.</returns>
    string ISale.Id => Id.ToString();
    
    /// <summary>
    /// Gets the Sale Number.
    /// </summary>
    public int SaleNumber { get; set; } = 0;
    
    /// <summary>
    /// Gets the Sale Number.
    /// </summary>
    public DateTime Date { get; set; }
    
    /// <summary>
    /// Gets the Client Id.
    /// </summary>
    public Guid ClienteId { get; set; } = Guid.Empty;
    
    /// <summary>
    /// Gets the Total Sale Amount.
    /// </summary>
    public decimal TotalSaleAmount { get; set; } = 0m;
    
    /// <summary>
    /// Gets the Branch.
    /// </summary>
    public string Branch { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets the Status of Sale.
    /// </summary>
    public SaleStatus Status { get; set; }
    
    /// <summary>
    /// Gets the Items of Sale.
    /// </summary>
    public IEnumerable<ItemSale>? Items { get; set; }
    
    /// <summary>
    /// Gets the date and time when the sale was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets the date and time of the last update to the sale's information.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
    
    /// <summary>
    /// Initializes a new instance of the Item Sale class.
    /// </summary>
    public Sale()
    {
        CreatedAt = DateTime.UtcNow;
    }
    
    /// <summary>
    /// Performs validation of the user entity using the SaleValidator rules.
    /// </summary>
    /// <returns>
    /// A <see cref="ValidationResultDetail"/> containing:
    /// - IsValid: Indicates whether all validation rules passed
    /// - Errors: Collection of validation errors if any rules failed
    /// </returns>
    /// <remarks>
    /// <listheader>The validation includes checking:</listheader>
    /// <list type="bullet">SaleNumber Value</list>
    /// <list type="bullet">Date of Sale Value</list>
    /// <list type="bullet">Cliente Id Value</list>
    /// <list type="bullet">Total Sale Amount Value</list>
    /// <list type="bullet">Branch Value</list>
    /// <list type="bullet">Items List</list>
    /// 
    /// </remarks>
    public ValidationResultDetail Validate()
    {
        var validator = new SaleValidator();
        var result = validator.Validate(this);
        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(o => (ValidationErrorDetail)o)
        };
    }
}