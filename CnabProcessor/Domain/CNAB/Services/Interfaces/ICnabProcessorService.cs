
using CnabProcessor.Models.Response;

namespace CnabProcessor.Domain.CNAB.Services.Interfaces;

public interface ICnabProcessorService
{
    Task<ProcessCNABResponse> ProcessCnabFileAsync(Stream fileStream);
}