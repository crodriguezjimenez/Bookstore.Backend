using MediatR;
using OrderService.Application.Common;
using OrderService.Application.Common.Events;
using OrderService.Application.Common.Interfaces;
using OrderService.Application.DTOs;
using OrderService.Domain.Entities;

namespace OrderService.Application.Commands;


public record OrderItemCommandDto(int BookId, string BookTitle, int Quantity, decimal UnitPrice);

public record CreateOrderMessageBusCommand(string CustomerEmail, string Branch, List<OrderItemCommandDto> Items) : IRequest<int>;

public record CreateOrderCommand(string Branch, OrderDto Order) : IRequest<int>;


public record CreateOrdersBatchCommand(string Branch, List<OrderDto> Orders) : IRequest<List<int>>;


public record UpdateOrderCommand(string Branch, int Id, OrderDto Order) : IRequest<bool>;


public record UpdateOrdersBatchCommand(string Branch, List<OrderItemDto> Orders) : IRequest<bool>;


public record DeleteOrderCommand(string Branch, int Id) : IRequest<bool>;


public record DeleteOrdersBatchCommand(string Branch, List<int> Ids) : IRequest<int>;





