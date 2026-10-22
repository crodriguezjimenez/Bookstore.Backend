using Microsoft.EntityFrameworkCore;

namespace OrderService.Application.Common.Interfaces;

public interface IOrderDbContextFactory
{
    DbContext CreateDbContext(string branchName);
}
