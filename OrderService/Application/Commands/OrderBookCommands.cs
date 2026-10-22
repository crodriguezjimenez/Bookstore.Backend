using OrderService.Application.DTOs;
using MediatR;

namespace OrderService.Application.Commands;


public record CreateOrderBookCommand(string Branch, OrderBookDto Book) : IRequest<int>;


public record CreateOrderBooksBatchCommand(string Branch, List<OrderBookDto> Books) : IRequest<List<int>>;


public record UpdateOrderBookCommand(string Branch, int Id, OrderBookDto Book) : IRequest<bool>;


public record UpdateOrderBooksBatchCommand(string Branch, List<OrderBookItemDto> Books) : IRequest<bool>;


public record DeleteOrderBookCommand(string Branch, int Id) : IRequest<bool>;


public record DeleteOrderBooksBatchCommand(string Branch, List<int> Ids) : IRequest<int>;