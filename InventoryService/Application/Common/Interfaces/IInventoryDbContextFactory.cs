using Microsoft.EntityFrameworkCore;

namespace InventoryService.Application.Common.Interfaces;

public interface IInventoryDbContextFactory
{
    DbContext CreateDbContext(string branchName);
}
