namespace Ambev.DeveloperEvaluation.Domain.Events;

public class PurchaseChangeEventArgs : EventArgs
{
    public Guid SaleId { get; }
    public DateTime Data { get; }

    public PurchaseChangeEventArgs(Guid saleId, DateTime data)
    {
        SaleId = saleId;
        Data = data;
    }
}