using OrderService.Application.DTOs;
using OrderService.Domain.Entities;

namespace OrderService.Application.Common.Interfaces
{
    public interface IOrderRepository
    {
        Task<List<OrderReadDto>> GetBooksFromBranchAsync(string branchName, CancellationToken cancellationToken = default);
        Task<OrderReadDto?> GetByIdAsync(string branchName, int id, CancellationToken cancellationToken = default);
        Task<int> AddAsync(string branchName, Order order, CancellationToken cancellationToken = default);
        Task<List<int>> AddRangeAsync(string branchName, List<Order> orders, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(string branchName, Order order, CancellationToken cancellationToken = default);
        Task<bool> UpdateRangeAsync(string branchName, List<Order> orders, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(string branchName, Order order, CancellationToken cancellationToken = default);
        Task<int> DeleteRangeAsync(string branchName, List<int> orderIds, CancellationToken cancellationToken = default);
    }
}
