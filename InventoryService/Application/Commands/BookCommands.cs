using InventoryService.Application.DTOs;
using MediatR;

namespace InventoryService.Application.Commands;


public record CreateBookCommand(string Branch, BookDto Book) : IRequest<int>;


public record CreateBooksBatchCommand(string Branch, List<BookDto> Books) : IRequest<List<int>>;


public record UpdateBookCommand(string Branch, int Id, BookDto Book) : IRequest<bool>;


public record UpdateBooksBatchCommand(string Branch, List<BookItemDto> Books) : IRequest<bool>;


public record DeleteBookCommand(string Branch, int Id) : IRequest<bool>;


public record DeleteBooksBatchCommand(string Branch, List<int> Ids) : IRequest<int>;