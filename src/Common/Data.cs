namespace Common;

public class Order
{
    public int Id { get; set; }
    public string ProductId { get; set; }
    public int Quantity { get; set; }
    public OrderStatus Status { get; set; }
    
    public enum OrderStatus
    {
        Pending,
        Completed,
    }
}