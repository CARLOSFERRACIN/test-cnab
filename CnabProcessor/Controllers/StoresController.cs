using Microsoft.AspNetCore.Mvc;
using CnabProcessor.Domain.CNAB.Services.Interfaces;
using CnabProcessor.Models.Response;

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
    
    /// <summary>
    /// Get all stores
    /// </summary>
    /// <returns>List of stores with summary information including balance and transaction count</returns>
    /// <response code="200">Stores retrieved successfully</response>
    /// <response code="500">Internal server error</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<StoreSummaryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
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
    
    /// <summary>
    /// Get store transactions
    /// </summary>
    /// <param name="storeId">Store ID to get transactions for</param>
    /// <returns>List of transactions for the specified store</returns>
    /// <response code="200">Transactions retrieved successfully</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("{storeId}/transactions")]
    [ProducesResponseType(typeof(List<TransactionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
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
