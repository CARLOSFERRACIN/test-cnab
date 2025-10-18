using CnabProcessor.Models.Entity;
using CnabProcessor.Repositories.Data;
using CnabProcessor.Repositories.Interfaces;
using System.Diagnostics.CodeAnalysis;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

namespace CnabProcessor.Repositories;

[ExcludeFromCodeCoverage]
public class TransactionRepository : Repository<Transaction>, ITransactionRepository
{
    public TransactionRepository(CnabContext context) : base(context)
    {
    }
    
    public async Task BulkInsertAsync(IEnumerable<Transaction> transactions)
    {
        await _context.BulkInsertAsync(transactions);
    }
    
    public async Task<List<Transaction>> GetByStoreIdAsync(int storeId)
    {
        return await _context.Transactions
            .Where(t => t.StoreId == storeId)
            .OrderBy(t => t.Date)
            .ToListAsync();
    }
}
