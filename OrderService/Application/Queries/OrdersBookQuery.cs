using MediatR;
using OrderService.Application.DTOs;

namespace OrderService.Application.Orders.Queries;


public record GetOrderBooksQuery(List<string> Branches) : IRequest<List<OrderBookReadDto>>;
public record GetOrderBookByIdQuery(string Branch, int Id) : IRequest<OrderBookReadDto?>;
