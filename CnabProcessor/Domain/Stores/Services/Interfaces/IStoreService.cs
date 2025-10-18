using CnabProcessor.Models.Entity;
using CnabProcessor.Models.Response;

namespace CnabProcessor.Domain.Stores.Services.Interfaces;

public interface IStoreService
{
    Task<List<StoreSummaryResponse>> GetStoreSummariesAsync();
    Task<List<Transaction>> GetStoreTransactionsAsync(int storeId);
}
