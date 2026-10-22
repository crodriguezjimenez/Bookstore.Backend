namespace OrderService.Domain.Entities;

public class OrderBook
{
    public int Id { get; set; }
    public int BookId { get; set; } 
    public string BookTitle { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
