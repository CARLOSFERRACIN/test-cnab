using CnabProcessor.Models.Entity;

namespace CnabProcessor.Repositories.Interfaces;

public interface IStoreRepository : IRepository<Store>
{
    Task<Store?> GetByOwnerAndNameAsync(string owner, string name);
    Task<IEnumerable<Store>> GetStoresWithTransactionsAsync();
}
