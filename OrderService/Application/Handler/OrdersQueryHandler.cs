using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderService.Application.Commands;
using OrderService.Application.Commands.Delegates;
using OrderService.Application.Common.Events;
using OrderService.Application.Common.Interfaces;
using OrderService.Application.DTOs;
using OrderService.Application.Orders.Queries;
using OrderService.Domain.Entities;
using System.Text.Json;

namespace OrderService.Application.Handler;

public class OrdersQueryHandler : IRequestHandler<GetOrdersQuery, List<OrderReadDto>>,
    IRequestHandler<GetOrderByIdQuery, OrderReadDto?>,
    IRequestHandler<CreateOrderCommand, int>,
    IRequestHandler<CreateOrdersBatchCommand, List<int>>,
    IRequestHandler<UpdateOrderCommand, bool>,
    IRequestHandler<UpdateOrdersBatchCommand, bool>,
    IRequestHandler<DeleteOrderCommand, bool>,
    IRequestHandler<DeleteOrdersBatchCommand, int>

{
    private readonly IOrderRepository _orderRepository;
    private readonly IMessageBusPublisher _messageBusPublisher;

    public OrdersQueryHandler(IOrderRepository orderRepository, IMessageBusPublisher messageBusPublisher)
    {
        _orderRepository = orderRepository;
        _messageBusPublisher = messageBusPublisher;
    }


    public async Task<List<OrderReadDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var tasks = new List<Task<List<OrderReadDto>>>();
        var branchesToQuery = request.Branches.Any()
            ? request.Branches
            : new List<string> { "Norte", "Este", "Oeste" };

        foreach (var branch in branchesToQuery)
        {
            tasks.Add(GetOrdersFromBranchAsync(branch));
        }

        var results = await Task.WhenAll(tasks);
        var finalOrdersList = results.SelectMany(r => r).ToList();


        AuditHandler auditPipeline = AuditServices.LogToConsole;
        auditPipeline += AuditServices.SendToTelemetry;

        string branchesString = string.Join(", ", branchesToQuery);
        auditPipeline.Invoke(branchesString, finalOrdersList.Count);

        // ==========================================

        return finalOrdersList;
    }

    private async Task<List<OrderReadDto>> GetOrdersFromBranchAsync(string branchName)
    {
        return await _orderRepository.GetBooksFromBranchAsync(branchName);
    }

    public async Task<OrderReadDto?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        return await _orderRepository.GetByIdAsync(request.Branch, request.Id, cancellationToken);
    }


    public async Task<int> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        return await _orderRepository.AddAsync(request.Branch, CastDto<Order>(request.Order), cancellationToken);
    }

    public async Task<int> Handle(CreateOrderMessageBusCommand request, CancellationToken cancellationToken)
    {
        var orderId = await _orderRepository.AddAsync(request.Branch, new Order { 
            BranchName= request.Branch,
            CustomerEmail = request.CustomerEmail,
            Items = request.Items.Select(item => new OrderBook
            {
                BookId = item.BookId,
                BookTitle = item.BookTitle,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            }).ToList()
        }, cancellationToken);


        var eventoNegocio = new OrderCreatedEvent
        {
            OrderId = orderId,
            BranchName = request.Branch,
            CustomerEmail = request.CustomerEmail,
            OrderDate = DateTime.Now,
            Items = request.Items.Select(item => new OrderItemEvent
            {
                BookId = item.BookId,
                BookTitle = item.BookTitle,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            }).ToList()
        };

        await _messageBusPublisher.PublishAsync(eventoNegocio, "orden creada");

        return orderId;
    }

    public async Task<List<int>> Handle(CreateOrdersBatchCommand request, CancellationToken cancellationToken)
    {
        return await _orderRepository.AddRangeAsync(request.Branch, CastDto<List<Order>>(request.Orders), cancellationToken);
    }

    public async Task<bool> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {

        return await _orderRepository.UpdateAsync(request.Branch, CastDto<Order>(request.Order), cancellationToken);
    }

    public async Task<bool> Handle(UpdateOrdersBatchCommand request, CancellationToken cancellationToken)
    {
        return await _orderRepository.UpdateRangeAsync(request.Branch, CastDto<List<Order>>(request.Orders), cancellationToken);
    }


    public async Task<bool> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
    {
        return await _orderRepository.DeleteAsync(request.Branch, new Order { Id = request.Id }, cancellationToken);
    }

    public async Task<int> Handle(DeleteOrdersBatchCommand request, CancellationToken cancellationToken)
    {
        return await _orderRepository.DeleteRangeAsync(request.Branch, request.Ids, cancellationToken);
    }

    private T CastDto<T>(object obj)
    {
        string jsonString = JsonSerializer.Serialize(obj);
        var item = JsonSerializer.Deserialize<T>(jsonString);
        return item;
    }


}

