namespace OrderService.Application.Common.Events;

public class OrderCreatedEvent
{
    public int OrderId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public List<OrderItemEvent> Items { get; set; } = new();
}
