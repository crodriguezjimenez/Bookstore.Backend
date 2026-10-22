namespace OrderService.Application.DTOs;

public class OrderReadDto
{
    public int Id { get; set; }
    public string CustomerEmail { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public List<OrderBookReadDto> Items { get; set; } = new();
    public string Branch { get; set; } = string.Empty;
}
