using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderService.API.Filters;
using OrderService.Application.Commands;
using OrderService.Application.DTOs;
using OrderService.Application.Orders.Queries;
using OrderService.Filters;

namespace OrderService.API.Endpoints;

public static class OrdersEndpoints
{

    public static IEndpointRouteBuilder MapOrdersEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/orders")
                       .AddEndpointFilter<BranchValidationFilter>();


        group.MapGet("/", async (IMediator mediator, string? branches) =>
        {
            var branchList = string.IsNullOrWhiteSpace(branches) ? new List<string>() : branches.Split(',').Select(b => b.Trim()).ToList();

            var result = await mediator.Send(new GetOrdersQuery(branchList));
            return Results.Ok(result);
        })
        .RequireAuthorization("CanReadInventory")
        .WithSummary("Obtiene ordenes de una o varias sucursales en paralelo");

        group.MapGet("/{branch}/{id:int}", async (IMediator mediator, string branch, int id) =>
        {
            var result = await mediator.Send(new GetOrderByIdQuery(branch, id));
            return result is not null ? Results.Ok(result) : Results.NotFound(new { Message = $"Orden con ID {id} no encontrada en sucursal {branch}." });
        })
        .RequireAuthorization("CanReadInventory")
        .WithSummary("Obtiene un orden específico de una sucursal");




        group.MapPost("/{branch}", async (IMediator mediator, string branch, OrderDto dto) =>
        {
            var id = await mediator.Send(new CreateOrderCommand(branch, dto));
            return Results.Created($"/api/orders/{branch}/{id}", new { Id = id, Branch = branch, Message = "Orden creada exitosamente" });
        })
        .RequireAuthorization("CanWriteInventory")
        .AddEndpointFilter<ValidationFilter<OrderDto>>()
        .WithSummary("Crea un orden en una sucursal específica");

        group.MapPost("/{branch}/batch", async (IMediator mediator, string branch, List<OrderDto> dtos) =>
        {
            var ids = await mediator.Send(new CreateOrdersBatchCommand(branch, dtos));
            return Results.Created($"/api/orders/{branch}", new { InsertedIds = ids, Count = ids.Count, Branch = branch });
        })
        .RequireAuthorization("CanWriteInventory")
        .AddEndpointFilter<ValidationFilter<OrderDto>>()
        .WithSummary("Crea una lista de ordenes por lote en una sucursal");


        group.MapPut("/{branch}/{id:int}", async (IMediator mediator, string branch, int id, OrderDto dto) =>
                {
                    var success = await mediator.Send(new UpdateOrderCommand(branch, id, dto));
                    return success ? Results.Ok(new { Message = "Orden actualizada correctamente." }) : Results.NotFound();
                })
        .RequireAuthorization("CanWriteInventory")
        .AddEndpointFilter<ValidationFilter<OrderDto>>()
        .WithSummary("Actualiza un orden en una sucursal");


        group.MapPut("/{branch}/batch", async (IMediator mediator, string branch, List<OrderItemDto> dtos) =>
        {
            var success = await mediator.Send(new UpdateOrdersBatchCommand(branch, dtos));
            return success ? Results.Ok(new { Message = "Ordenes actualizadas correctamente en lote." }) : Results.BadRequest();
        })
        .RequireAuthorization("CanWriteInventory")
        .AddEndpointFilter<ValidationFilter<OrderItemDto>>()
        .WithSummary("Actualiza múltiples ordenes por lote en una sucursal");




        group.MapDelete("/{branch}/{id:int}", async (IMediator mediator, string branch, int id) =>
        {
            var success = await mediator.Send(new DeleteOrderCommand(branch, id));
            return success ? Results.Ok(new { Message = "Orden eliminada correctamente." }) : Results.NotFound();
        })
        .RequireAuthorization("CanWriteInventory")
        .WithSummary("Elimina un orden de una sucursal");



        group.MapDelete("/{branch}/batch", async (IMediator mediator, string branch, [FromBodyAttribute] List<int> ids) =>
        {
            var deletedCount = await mediator.Send(new DeleteOrdersBatchCommand(branch, ids));
            return Results.Ok(new { DeletedCount = deletedCount, Message = $"Se eliminaron {deletedCount} ordenes." });
        })
        .RequireAuthorization("CanWriteInventory")
        .WithSummary("Elimina una lista de ordenes por IDs en una sucursal");

        return app;
    }
}
