using InventoryService.Application.DTOs;
using InventoryService.Domain;


namespace InventoryService.Application.Common.Interfaces;

public interface IBookRepository
{
    Task<List<BookReadDto>> GetBooksFromBranchAsync(string branchName, CancellationToken cancellationToken = default);
    Task<BookReadDto?> GetByIdAsync(string branchName, int id, CancellationToken cancellationToken = default);
    Task<int> AddAsync(string branchName, Book book, CancellationToken cancellationToken = default);
    Task<List<int>> AddRangeAsync(string branchName, List<Book> books, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(string branchName, Book book, CancellationToken cancellationToken = default);
    Task<bool> UpdateRangeAsync(string branchName, List<Book> books, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string branchName, Book book, CancellationToken cancellationToken = default);
    Task<int> DeleteRangeAsync(string branchName, List<int> bookIds, CancellationToken cancellationToken = default);
}