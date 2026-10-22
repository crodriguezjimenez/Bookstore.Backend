namespace OrderService.Domain.Entities;

public class Order
{
    public int Id { get; set; }
    public string CustomerEmail { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    public string BranchName { get; set; } = string.Empty;

    public List<OrderBook> Items { get; set; } = new();

}
