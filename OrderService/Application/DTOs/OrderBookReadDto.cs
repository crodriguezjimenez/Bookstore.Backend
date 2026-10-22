namespace OrderService.Application.DTOs;

public class OrderBookReadDto
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public string Branch { get; set; } = string.Empty;
}