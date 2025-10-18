using CnabProcessor.Models.Entity;

namespace CnabProcessor.Repositories.Interfaces;

public interface ITransactionRepository : IRepository<Transaction>
{
    Task BulkInsertAsync(IEnumerable<Transaction> transactions);
    Task<List<Transaction>> GetByStoreIdAsync(int storeId);
}
