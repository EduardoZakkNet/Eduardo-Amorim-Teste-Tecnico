namespace Ambev.DeveloperEvaluation.Domain.Events;

public class PurchaseCreatedEventArgs : EventArgs
{
    public Guid SaleId { get; }
    public DateTime Data { get; }

    public PurchaseCreatedEventArgs(Guid saleId, DateTime data)
    {
        SaleId = saleId;
        Data = data;
    }
}