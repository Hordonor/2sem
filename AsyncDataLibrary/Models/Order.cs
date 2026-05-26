namespace AsyncDataLibrary.Models;

public enum OrderStatus
{
    Pending,
    Processing,
    Completed,
    Cancelled
}

[StorageFile("orders.json")]
public class Order : IEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public Guid BookId { get; set; }
    public int Quantity { get; set; } = 1;
    public decimal TotalPrice { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public override string ToString() =>
        $"Order [{Id:D}] | User: {UserId:D} | Book: {BookId:D} | Qty: {Quantity} | Total: {TotalPrice:C} | Status: {Status}";
}
