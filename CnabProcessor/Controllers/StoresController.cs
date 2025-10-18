using Microsoft.AspNetCore.Mvc;
using CnabProcessor.Domain.CNAB.Services.Interfaces;

namespace CnabProcessor.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StoresController : ControllerBase
{
    private readonly ICnabProcessorService _processorService;
    private readonly ILogger<StoresController> _logger;
    
    public StoresController(ICnabProcessorService processorService, ILogger<StoresController> logger)
    {
        _processorService = processorService;
        _logger = logger;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetStores()
    {
        try
        {
            var stores = await _processorService.GetStoreSummariesAsync();
            return Ok(stores);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving stores");
            return StatusCode(500, "Internal server error while retrieving stores.");
        }
    }
    
    [HttpGet("{storeId}/transactions")]
    public async Task<IActionResult> GetStoreTransactions(int storeId)
    {
        try
        {
            var transactions = await _processorService.GetStoreTransactionsAsync(storeId);
            return Ok(transactions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving store transactions for store {StoreId}", storeId);
            return StatusCode(500, "Internal server error while retrieving store transactions.");
        }
    }
}
