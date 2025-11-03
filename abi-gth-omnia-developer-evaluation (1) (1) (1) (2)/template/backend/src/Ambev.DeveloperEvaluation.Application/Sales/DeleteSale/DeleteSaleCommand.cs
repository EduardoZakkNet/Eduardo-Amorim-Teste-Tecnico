using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;

/// <summary>
/// Command for deleting a Sale
/// </summary>
public class DeleteSaleCommand : IRequest<DeleteSaleResponse>
{
    /// <summary>
    /// The unique identifier of the Sale to delete
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Initializes a new instance of DeleteSaleCommand
    /// </summary>
    /// <param name="id">The ID of the Sale to delete</param>
    public DeleteSaleCommand(Guid id)
    {
        Id = id;
    }
    
    /// <summary>
    /// Initializes a new instance default of DeleteSaleCommand
    /// </summary>
    /// <param name="id">The ID of the Sale to delete</param>
    public DeleteSaleCommand()
    {
    }
}