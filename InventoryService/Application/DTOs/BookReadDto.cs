namespace InventoryService.Application.DTOs;

public class BookReadDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string Branch { get; set; } = string.Empty;
}