using InventoryService.API.Filters;
using InventoryService.Application.Commands;
using InventoryService.Application.Query;
using InventoryService.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.API.Endpoints;

public static class BookEndpoints
{
    public static IEndpointRouteBuilder MapBookEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/books")
                       .AddEndpointFilter<BranchValidationFilter>(); 


        group.MapGet("/", async (IMediator mediator, string? branches) =>
        {
            var branchList = string.IsNullOrWhiteSpace(branches) ? new List<string>() : branches.Split(',').Select(b => b.Trim()).ToList();

            var result = await mediator.Send(new GetBooksQuery(branchList));
            return Results.Ok(result);
        })
        .RequireAuthorization("CanReadInventory")
        .WithSummary("Obtiene libros de una o varias sucursales en paralelo");

        group.MapGet("/{branch}/{id:int}", async (IMediator mediator, string branch, int id) =>
        {
            var result = await mediator.Send(new GetBookByIdQuery(branch, id));
            return result is not null ? Results.Ok(result) : Results.NotFound(new { Message = $"Libro con ID {id} no encontrado en sucursal {branch}." });
        })
        .RequireAuthorization("CanReadInventory")
        .WithSummary("Obtiene un libro específico de una sucursal");




        group.MapPost("/{branch}", async (IMediator mediator, string branch, BookDto dto) =>
        {
            var id = await mediator.Send(new CreateBookCommand(branch, dto));
            return Results.Created($"/api/books/{branch}/{id}", new { Id = id, Branch = branch, Message = "Libro creado exitosamente" });
        })
        .RequireAuthorization("CanWriteInventory")
        .AddEndpointFilter<ValidationFilter<BookDto>>()
        .WithSummary("Crea un libro en una sucursal específica");

        group.MapPost("/{branch}/batch", async (IMediator mediator, string branch, List<BookDto> dtos) =>
        {
            var ids = await mediator.Send(new CreateBooksBatchCommand(branch, dtos));
            return Results.Created($"/api/books/{branch}", new { InsertedIds = ids, Count = ids.Count, Branch = branch });
        })
        .RequireAuthorization("CanWriteInventory")
        .AddEndpointFilter<ValidationFilter<BookDto>>()
        .WithSummary("Crea una lista de libros por lote en una sucursal");




        group.MapPut("/{branch}/{id:int}", async (IMediator mediator, string branch, int id, BookDto dto) =>
        {
            var success = await mediator.Send(new UpdateBookCommand(branch, id, dto));
            return success ? Results.Ok(new { Message = "Libro actualizado correctamente." }) : Results.NotFound();
        })
        .RequireAuthorization("CanWriteInventory")
        .AddEndpointFilter<ValidationFilter<BookDto>>()
        .WithSummary("Actualiza un libro en una sucursal");


        group.MapPut("/{branch}/batch", async (IMediator mediator, string branch, List<BookItemDto> dtos) =>
        {
            var success = await mediator.Send(new UpdateBooksBatchCommand(branch, dtos));
            return success ? Results.Ok(new { Message = "Libros actualizados correctamente en lote." }) : Results.BadRequest();
        })
        .RequireAuthorization("CanWriteInventory")
        .AddEndpointFilter<ValidationFilter<BookDto>>()
        .WithSummary("Actualiza múltiples libros por lote en una sucursal");




        group.MapDelete("/{branch}/{id:int}", async (IMediator mediator, string branch, int id) =>
        {
            var success = await mediator.Send(new DeleteBookCommand(branch, id));
            return success ? Results.Ok(new { Message = "Libro eliminado correctamente." }) : Results.NotFound();
        })
        .RequireAuthorization("CanWriteInventory")
        .WithSummary("Elimina un libro de una sucursal");



        group.MapDelete("/{branch}/batch", async (IMediator mediator, string branch, [FromBodyAttribute] List<int> ids) =>
        {
            var deletedCount = await mediator.Send(new DeleteBooksBatchCommand(branch, ids));
            return Results.Ok(new { DeletedCount = deletedCount, Message = $"Se eliminaron {deletedCount} libros." });
        })
        .RequireAuthorization("CanWriteInventory")
        .WithSummary("Elimina una lista de libros por IDs en una sucursal");

        return app;
    }
}
