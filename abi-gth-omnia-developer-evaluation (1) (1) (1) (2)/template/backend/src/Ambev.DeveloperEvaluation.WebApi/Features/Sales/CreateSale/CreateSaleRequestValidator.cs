using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

/// <summary>
/// Validator for CreateSaleRequest that defines validation rules for user creation.
/// </summary>
public class CreateSaleRequestValidator : AbstractValidator<CreateSaleRequest>
{
    /// <summary>
    /// Initializes a new instance of the CreateUserRequestValidator with defined validation rules.
    /// </summary>
    /// <remarks>
    /// Validation rules include:
    /// - Description: Required, length between 100 and 200 characters
    /// - Quantity: Required
    /// - Unit Value: Required and greater than 0
    /// - Discount: Required and greater than 0
    /// - SaleNumber: Required and greater than 0
    /// - Date: Required
    /// - Branch: Required
    /// </remarks>
    public CreateSaleRequestValidator()
    {
        RuleFor(sale => sale.Items).Must(item => item.Any())
            .NotNull().WithMessage("The Items list cannot be null.");
        
        RuleFor(sale => sale.SaleNumber)
            .NotNull().WithMessage("Sale Number is required.")
            .GreaterThan(0).WithMessage("Sale Number must be greater than 0.");

        RuleFor(sale => sale.Date)
            .NotNull().WithMessage("The Date of Sale cannot be null.");

        RuleFor(sale => sale.Branch)
            .NotNull().WithMessage("The Branch cannot be null.");
        
        RuleForEach(sale => sale.Items)
            .NotNull().WithMessage("Item is required.")
            .Must(item => !string.IsNullOrWhiteSpace(item.Description))
            .WithMessage("Description is required.")
            .Must(item => item.Description.Length >= 100 && item.Description.Length <= 200)
            .WithMessage("Description must be between 100 and 200 characters.");
        
        RuleForEach(sale => sale.Items)
            .ChildRules(items => 
            {
                items.RuleFor(item => item.Quantity)
                    .NotNull().WithMessage("Quantity is required.")
                    .GreaterThan(0).WithMessage("Quantity must be greater than 0.");
            });
        
        RuleForEach(sale => sale.Items)
            .ChildRules(items => 
            {
                items.RuleFor(item => item.UnitValue)
                    .NotNull().WithMessage("Unit Value is required.")
                    .GreaterThan(0).WithMessage("Unit Value must be greater than 0.");
            });
        
        RuleForEach(sale => sale.Items)
            .ChildRules(items => 
            {
                items.RuleFor(item => item.Discount)
                    .NotNull().WithMessage("Discount is required.")
                    .GreaterThan(0).WithMessage("Discount must be greater than 0.");
            });
    }
}