using InventoryService.Application.DTOs;
using MediatR;

namespace InventoryService.Application.Query;

public record GetBooksQuery(List<string> Branches) : IRequest<List<BookReadDto>>;

public record GetBookByIdQuery(string Branch, int Id) : IRequest<BookReadDto?>;

