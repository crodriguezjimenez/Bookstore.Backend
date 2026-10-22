using Microsoft.EntityFrameworkCore;
using OrderService.Application.Common.Interfaces;
using OrderService.Application.DTOs;
using OrderService.Domain.Entities;

namespace OrderService.Infrastructure.Repositories
{
    public class OrderRepository: IOrderRepository
    {
        private readonly IOrderDbContextFactory _dbContextFactory;

        public OrderRepository(IOrderDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<List<OrderReadDto>> GetBooksFromBranchAsync(string branchName, CancellationToken cancellationToken = default)
        {
            try
            {

                using var context = _dbContextFactory.CreateDbContext(branchName);

                return await context.Set<Order>()
                .AsNoTracking()
                .Select(b => new OrderReadDto
                {
                    Id = b.Id,
                    CustomerEmail = b.CustomerEmail,
                    OrderDate = b.OrderDate,
                    TotalAmount = b.Items.Sum(i => i.UnitPrice * i.Quantity),
                    Items = b.Items.Select(i => new OrderBookReadDto
                    {
                        Id = i.Id,
                        BookId = i.BookId,
                        BookTitle = i.BookTitle,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice,
                        TotalPrice = i.UnitPrice * i.Quantity
                    }).ToList(),
                    Branch = branchName
                })
                .ToListAsync();
            }
            catch (Exception)
            {
                return new List<OrderReadDto>();
            }
        }
        public async Task<OrderReadDto?> GetByIdAsync(string branchName, int id, CancellationToken cancellationToken = default)
        {
            using var context = _dbContextFactory.CreateDbContext(branchName);
            var order = await context.Set<Order>().AsNoTracking()
           .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

            if (order == null) return null;

            return new OrderReadDto
            {
                Id = order.Id,
                CustomerEmail = order.CustomerEmail,
                OrderDate = order.OrderDate,
                TotalAmount = order.Items.Sum(i => i.UnitPrice * i.Quantity),
                Items = order.Items.Select(i => new OrderBookReadDto
                {
                    Id = i.Id,
                    BookId = i.BookId,
                    BookTitle = i.BookTitle,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    TotalPrice = i.UnitPrice * i.Quantity
                }).ToList(),
                Branch = branchName
            };
        }

        public async Task<int> AddAsync(string branchName, Order order, CancellationToken cancellationToken = default)
        {
            using var context = _dbContextFactory.CreateDbContext(branchName);
            await context.Set<Order>().AddAsync(order, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            return order.Id;
        }

        public async Task<List<int>> AddRangeAsync(string branchName, List<Order> orders, CancellationToken cancellationToken = default)
        {
            using var context = _dbContextFactory.CreateDbContext(branchName);
            await context.Set<Order>().AddRangeAsync(orders, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);


            return orders.Select(b => b.Id).ToList();
        }

        public async Task<bool> UpdateAsync(string branchName, Order order, CancellationToken cancellationToken = default)
        {
            using var context = _dbContextFactory.CreateDbContext(branchName);
            var item = await context.Set<Order>().FindAsync(new object[] { order.Id }, cancellationToken);
            if (order == null) return false;

            item.CustomerEmail = order.CustomerEmail;
            item.OrderDate = order.OrderDate;
            item.BranchName = order.BranchName;

            await context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> UpdateRangeAsync(string branchName, List<Order> orders, CancellationToken cancellationToken = default)
        {
            using var context = _dbContextFactory.CreateDbContext(branchName);
            var ids = orders.Select(x => x.Id).ToList();

            var existingOrders = await context.Set<Order>()
                .Where(b => ids.Contains(b.Id))
                .ToListAsync(cancellationToken);

            foreach (var item in orders)
            {
                var order = existingOrders.FirstOrDefault(b => b.Id == item.Id);
                if (order != null)
                {
                    order.CustomerEmail = item.CustomerEmail;
                    order.OrderDate = item.OrderDate;
                    order.BranchName = item.BranchName;
                }
            }

            await context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> DeleteAsync(string branchName, Order order, CancellationToken cancellationToken = default)
        {
            using var context = _dbContextFactory.CreateDbContext(branchName);
            var item = await context.Set<Order>().FindAsync(new object[] { order.Id }, cancellationToken);
            if (item == null) return false;

            context.Set<Order>().Remove(item);
            await context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<int> DeleteRangeAsync(string branchName, List<int> orders, CancellationToken cancellationToken = default)
        {
            using var context = _dbContextFactory.CreateDbContext(branchName);
            var ordersToDelete = await context.Set<Order>()
            .Where(b => orders.Contains(b.Id)) 
            .ToListAsync(cancellationToken);

            if (!ordersToDelete.Any()) return 0;

            context.Set<Order>().RemoveRange(ordersToDelete);
            return await context.SaveChangesAsync(cancellationToken);
        }

    }

}
