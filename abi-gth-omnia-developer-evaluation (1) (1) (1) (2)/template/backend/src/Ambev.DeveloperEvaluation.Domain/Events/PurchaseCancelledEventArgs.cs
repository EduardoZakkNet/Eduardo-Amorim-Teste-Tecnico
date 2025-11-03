namespace Ambev.DeveloperEvaluation.Domain.Events;

public class PurchaseCancelledEventArgs : EventArgs
{
    public Guid SaleId { get; }
    public DateTime Data { get; }

    public PurchaseCancelledEventArgs(Guid saleId, DateTime data)
    {
        SaleId = saleId;
        Data = data;
    }
}