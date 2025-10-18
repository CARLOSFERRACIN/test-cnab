using Microsoft.AspNetCore.Mvc;
using CnabProcessor.Domain.CNAB.Services.Interfaces;

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
    
    [HttpPost("upload")]
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


