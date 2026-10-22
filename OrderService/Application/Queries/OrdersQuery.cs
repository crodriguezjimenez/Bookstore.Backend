using MediatR;
using OrderService.Application.DTOs;

namespace OrderService.Application.Orders.Queries;


public record GetOrdersQuery(List<string> Branches) : IRequest<List<OrderReadDto>>;
public record GetOrderByIdQuery(string Branch, int Id) : IRequest<OrderReadDto?>;
