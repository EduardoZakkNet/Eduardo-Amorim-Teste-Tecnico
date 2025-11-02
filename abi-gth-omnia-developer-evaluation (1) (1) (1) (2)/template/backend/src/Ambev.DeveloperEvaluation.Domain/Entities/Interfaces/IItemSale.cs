namespace Ambev.DeveloperEvaluation.Domain.Entities.Interfaces;

/// <summary>
/// Defines the contract for representing a Sales Item in the system.
/// </summary>
public interface IItemSale
{
    /// <summary>
    /// Gets the unique identifier of the Sales Item.
    /// </summary>
    /// <returns>The ID of the Sales Item as a string.</returns>
    public string Id { get; }
}