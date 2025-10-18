using CnabProcessor.Domain.Stores.Services.Interfaces;
using CnabProcessor.Models.Response;
using Microsoft.AspNetCore.Mvc;

namespace CnabProcessor.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StoresController : ControllerBase
{
    private readonly IStoreService _storeService;
    private readonly ILogger<StoresController> _logger;

    public StoresController(IStoreService storeService, ILogger<StoresController> logger)
    {
        _storeService = storeService;
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
            var stores = await _storeService.GetStoreSummariesAsync();
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
            var transactions = await _storeService.GetStoreTransactionsAsync(storeId);
            return Ok(transactions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving store transactions for store {StoreId}", storeId);
            return StatusCode(500, "Internal server error while retrieving store transactions.");
        }
    }
}
