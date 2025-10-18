
using CnabProcessor.Models.Response;
using CnabProcessor.Models.Entity;

namespace CnabProcessor.Domain.CNAB.Services.Interfaces;

public interface ICnabProcessorService
{
    Task<ProcessCNABResponse> ProcessCnabFileAsync(Stream fileStream);
    Task<List<StoreSummaryResponse>> GetStoreSummariesAsync();
    Task<List<CnabProcessor.Models.Entity.Transaction>> GetStoreTransactionsAsync(int storeId);
}