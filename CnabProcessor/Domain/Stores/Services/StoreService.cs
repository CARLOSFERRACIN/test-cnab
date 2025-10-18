using CnabProcessor.Domain.Stores.Services.Interfaces;
using CnabProcessor.Models.Entity;
using CnabProcessor.Models.Response;
using CnabProcessor.Repositories.Interfaces;

namespace CnabProcessor.Domain.Stores.Services;

public class StoreService : IStoreService
{
    private readonly IUnitOfWork _unitOfWork;

    public StoreService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<StoreSummaryResponse>> GetStoreSummariesAsync()
    {
        var stores = await _unitOfWork.Stores.GetStoresWithTransactionsAsync();

        return stores.Select(store => new StoreSummaryResponse
        {
            Id = store.Id,
            Owner = store.Owner,
            Name = store.Name,
            Balance = store.Balance,
            TransactionCount = store.Transactions.Count
        }).ToList();
    }

    public async Task<List<Transaction>> GetStoreTransactionsAsync(int storeId)
    {
        var transactions = await _unitOfWork.Transactions.GetByStoreIdAsync(storeId);
        return transactions;
    }
}
