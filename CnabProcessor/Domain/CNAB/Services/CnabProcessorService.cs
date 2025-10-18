using CnabProcessor.Domain.CNAB.Services.Interfaces;
using CnabProcessor.Models.Entity;
using CnabProcessor.Models.Response;
using CnabProcessor.Repositories.Interfaces;

namespace CnabProcessor.Domain.CNAB.Services;

public class CnabProcessorService : ICnabProcessorService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICnabParserService _parserService;

    public CnabProcessorService(IUnitOfWork unitOfWork, ICnabParserService parserService)
    {
        _unitOfWork = unitOfWork;
        _parserService = parserService;
    }

    public async Task<ProcessCNABResponse> ProcessCnabFileAsync(Stream fileStream)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var transactions = await _parserService.ParseCnabFileAsync(fileStream);

            if (!transactions.Any())
            {
                await _unitOfWork.RollbackTransactionAsync();
                return new ProcessCNABResponse { Success = false, Message = "No valid transactions found in the file." };
            }

            var storeGroups = transactions.GroupBy(t => new { t.StoreOwner, t.StoreName });

            var processedStores = 0;
            var processedTransactions = 0;
            var allTransactions = new List<Transaction>();

            // Process stores and prepare transactions for bulk insert
            foreach (var storeGroup in storeGroups)
            {
                var store = await _unitOfWork.Stores.GetByOwnerAndNameAsync(storeGroup.Key.StoreOwner, storeGroup.Key.StoreName);

                if (store == null)
                {
                    store = new Store
                    {
                        Owner = storeGroup.Key.StoreOwner,
                        Name = storeGroup.Key.StoreName
                    };
                    await _unitOfWork.Stores.AddAsync(store);
                    await _unitOfWork.SaveChangesAsync();
                    processedStores++;
                }

                // Prepare transactions for bulk insert
                foreach (var transaction in storeGroup)
                {
                    transaction.StoreId = store.Id;
                    allTransactions.Add(transaction);
                    processedTransactions++;
                }
            }

            // Bulk insert all transactions at once - MUCH FASTER!
            if (allTransactions.Any())
            {
                await _unitOfWork.Transactions.BulkInsertAsync(allTransactions);
            }

            await _unitOfWork.CommitTransactionAsync();

            return new ProcessCNABResponse
            {
                Success = true,
                Message = $"File processed successfully. {processedStores} stores processed, {processedTransactions} transactions imported.",
                ProcessedStores = processedStores,
                ProcessedTransactions = processedTransactions
            };
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            return new ProcessCNABResponse
            {
                Success = false,
                Message = $"Error processing file: {ex.Message}"
            };
        }
    }

}
