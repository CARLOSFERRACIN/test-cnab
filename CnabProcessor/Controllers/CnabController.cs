using CnabProcessor.Domain.CNAB.Services.Interfaces;
using CnabProcessor.Models.Response;
using Microsoft.AspNetCore.Mvc;

namespace CnabProcessor.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CnabController : ControllerBase
{
    private readonly ICnabProcessorService _processorService;
    private readonly ILogger<CnabController> _logger;

    public CnabController(ICnabProcessorService processorService, ILogger<CnabController> logger)
    {
        _processorService = processorService;
        _logger = logger;
    }

    /// <summary>
    /// Upload CNAB file
    /// </summary>
    /// <param name="file">CNAB file to upload (.txt format)</param>
    /// <returns>Processing result with store and transaction counts</returns>
    /// <response code="200">File processed successfully</response>
    /// <response code="400">Invalid file or processing error</response>
    /// <response code="500">Internal server error</response>
    [HttpPost("upload")]
    [ProducesResponseType(typeof(ProcessCNABResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProcessCNABResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UploadCnabFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file was uploaded.");
        }

        if (!file.FileName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest("The file must be a text file (.txt).");
        }

        try
        {
            using var stream = file.OpenReadStream();
            var result = await _processorService.ProcessCnabFileAsync(stream);

            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing CNAB file");
            return StatusCode(500, "Internal server error while processing the file.");
        }
    }

}


