namespace OrderService.Application.DTOs;

public class OrderDto
{
    public string CustomerEmail { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public string Branch { get; set; } = string.Empty; 
    public decimal TotalAmount { get; set; }
    public List<OrderBookItemDto> Items { get; set; } = new();
}
