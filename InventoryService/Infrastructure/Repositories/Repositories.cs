using InventoryService.Application.Common.Interfaces;
using InventoryService.Application.DTOs;
using InventoryService.Domain;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Infrastructure.Repositories;

public class BookRepository : IBookRepository
{
    private readonly IInventoryDbContextFactory _dbContextFactory;

    public BookRepository(IInventoryDbContextFactory dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }
    
    public async Task<List<BookReadDto>> GetBooksFromBranchAsync(string branchName, CancellationToken cancellationToken = default)
    {
        try
        {

            using var context = _dbContextFactory.CreateDbContext(branchName);

            return await context.Set<Book>()
                .AsNoTracking()
                .Select(b => new BookReadDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Author = b.Author,
                    Price = b.Price,
                    Stock = b.Stock,
                    Branch = branchName
                })
                .ToListAsync(cancellationToken);
        }
        catch (Exception)
        {
            return new List<BookReadDto>();
        }
    }
    public async Task<BookReadDto?> GetByIdAsync(string branchName, int id, CancellationToken cancellationToken = default)
    {
        using var context = _dbContextFactory.CreateDbContext(branchName);
        var book = await context.Set<Book>().AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

        if (book == null) return null;

        return new BookReadDto
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            Price = book.Price,
            Stock = book.Stock,
            Branch = branchName
        };
    }

    public async Task<int> AddAsync(string branchName, Book book, CancellationToken cancellationToken = default)
    {
        using var context = _dbContextFactory.CreateDbContext(branchName);
        await context.Set<Book>().AddAsync(book, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return book.Id;
    }

    public async Task<List<int>> AddRangeAsync(string branchName, List<Book> books, CancellationToken cancellationToken = default)
    {
        using var context = _dbContextFactory.CreateDbContext(branchName);
        await context.Set<Book>().AddRangeAsync(books, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);


        return books.Select(b => b.Id).ToList();
    }

    public async Task<bool> UpdateAsync(string branchName, Book book, CancellationToken cancellationToken = default)
    {
        using var context = _dbContextFactory.CreateDbContext(branchName);
        var item = await context.Set<Book>().FindAsync(new object[] { book.Id }, cancellationToken);
        if (item == null) return false;

        item.Title = book.Title;
        item.Author = book.Author;
        item.Price = book.Price;
        item.Stock = book.Stock;

        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> UpdateRangeAsync(string branchName, List<Book> books, CancellationToken cancellationToken = default)
    {
        using var context = _dbContextFactory.CreateDbContext(branchName);
        var items = books.Select(x => x.Id).ToList();

        var existingBooks = await context.Set<Book>()
            .Where(b => items.Contains(b.Id))
            .ToListAsync(cancellationToken);

        foreach (var item in books)
        {
            var book = existingBooks.FirstOrDefault(b => b.Id == item.Id);
            if (book != null)
            {
                book.Title = item.Title;
                book.Author = item.Author;
                book.Price = item.Price;
                book.Stock = item.Stock;
            }
        }

        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(string branchName, Book book, CancellationToken cancellationToken = default)
    {
        using var context = _dbContextFactory.CreateDbContext(branchName);
        var item = await context.Set<Book>().FindAsync(new object[] { book.Id }, cancellationToken);
        if (item == null) return false;

        context.Set<Book>().Remove(item);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<int> DeleteRangeAsync(string branchName, List<int> books, CancellationToken cancellationToken = default)
    {
        using var context = _dbContextFactory.CreateDbContext(branchName);
        var items = await context.Set<Book>()
            .Where(b => books.Contains(b.Id))
            .ToListAsync(cancellationToken);

        if (!items.Any()) return 0;

        context.Set<Book>().RemoveRange(items);
        return await context.SaveChangesAsync(cancellationToken);
    }

}
