namespace Ambev.DeveloperEvaluation.Domain.Entities.Interfaces;

/// <summary>
/// Defines the contract for representing a Sale in the system.
/// </summary>
public interface ISale
{
    /// <summary>
    /// Gets the unique identifier of the Sale.
    /// </summary>
    /// <returns>The ID of the Sale as a string.</returns>
    public string Id { get; }
    
    /// <summary>
    /// Gets the Status of the Sale.
    /// </summary>
    /// <returns>The Status of the Sale as a string.</returns>
    public string Status { get; }
}