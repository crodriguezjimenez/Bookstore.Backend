using InventoryService.Application.Commands;
using InventoryService.Application.Commands.Delegates;
using InventoryService.Application.Common.Interfaces;
using InventoryService.Application.DTOs;
using InventoryService.Application.Query;
using InventoryService.Domain;
using MediatR;
using System.Text.Json;

namespace InventoryService.Application.Handler;

public class BooksQueryHandler : IRequestHandler<GetBooksQuery, List<BookReadDto>>,
    IRequestHandler<GetBookByIdQuery, BookReadDto?>,
    IRequestHandler<CreateBookCommand, int>,
    IRequestHandler<CreateBooksBatchCommand, List<int>>,
    IRequestHandler<UpdateBookCommand, bool>,
    IRequestHandler<UpdateBooksBatchCommand, bool>,
    IRequestHandler<DeleteBookCommand, bool>,
    IRequestHandler<DeleteBooksBatchCommand, int>

{
    private readonly IBookRepository _bookRepository;

    public BooksQueryHandler(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }


    public async Task<List<BookReadDto>> Handle(GetBooksQuery request, CancellationToken cancellationToken)
    {
        var tasks = new List<Task<List<BookReadDto>>>();
        var branchesToQuery = request.Branches.Any() ? request.Branches : new List<string> { "Norte", "Este", "Oeste" };

        foreach (var branch in branchesToQuery)
        {
            tasks.Add(GetBooksFromBranchAsync(branch));
        }

        var results = await Task.WhenAll(tasks);
        var finalBooksList = results.SelectMany(r => r).ToList();


        AuditHandler auditPipeline = AuditServices.LogToConsole;
        auditPipeline += AuditServices.SendToTelemetry;

        string branchesString = string.Join(", ", branchesToQuery);
        auditPipeline.Invoke(branchesString, finalBooksList.Count);


        return finalBooksList;
    }

    private async Task<List<BookReadDto>> GetBooksFromBranchAsync(string branchName)
    {
        return await _bookRepository.GetBooksFromBranchAsync(branchName);
    }

    public async Task<BookReadDto?> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
    {
        return await _bookRepository.GetByIdAsync(request.Branch, request.Id, cancellationToken);
    }

    public async Task<int> Handle(CreateBookCommand request, CancellationToken cancellationToken)
    {
        return await _bookRepository.AddAsync(request.Branch, CastDto<Book>(request.Book), cancellationToken);
    }

    public async Task<List<int>> Handle(CreateBooksBatchCommand request, CancellationToken cancellationToken)
    {
        return await _bookRepository.AddRangeAsync(request.Branch, CastDto<List<Book>>(request.Books), cancellationToken);
    }

    public async Task<bool> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
    {
        return await _bookRepository.UpdateAsync(request.Branch, CastDto<Book>(request.Book), cancellationToken);
    }

    public async Task<bool> Handle(UpdateBooksBatchCommand request, CancellationToken cancellationToken)
    {
        return await _bookRepository.UpdateRangeAsync(request.Branch, CastDto<List<Book>>(request.Books), cancellationToken);
    }

    public async Task<bool> Handle(DeleteBookCommand request, CancellationToken cancellationToken)
    {
        return await _bookRepository.DeleteAsync(request.Branch, new Book { Id = request.Id }, cancellationToken);
    }

    public async Task<int> Handle(DeleteBooksBatchCommand request, CancellationToken cancellationToken)
    {
        return await _bookRepository.DeleteRangeAsync(request.Branch, request.Ids, cancellationToken);
    }

    private T CastDto<T>(object obj)
    {
        string jsonString = JsonSerializer.Serialize(obj);
        var item = JsonSerializer.Deserialize<T>(jsonString);
        return item;
    }
}
