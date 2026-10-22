namespace OrderService.Application.Common.Events;

public class OrderItemEvent
{
    public int BookId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
