using CnabProcessor.Models.Entity;
using CnabProcessor.Repositories.Data;
using CnabProcessor.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace CnabProcessor.Repositories;

[ExcludeFromCodeCoverage]
public class StoreRepository : Repository<Store>, IStoreRepository
{
    public StoreRepository(CnabContext context) : base(context)
    {
    }

    public async Task<Store?> GetByOwnerAndNameAsync(string owner, string name)
    {
        return await _dbSet
            .FirstOrDefaultAsync(s => s.Owner == owner && s.Name == name);
    }

    public async Task<IEnumerable<Store>> GetStoresWithTransactionsAsync()
    {
        return await _dbSet
            .Include(s => s.Transactions)
            .ToListAsync();
    }
}
