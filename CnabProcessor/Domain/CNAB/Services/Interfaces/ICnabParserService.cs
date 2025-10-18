using CnabProcessor.Models.Entity;

namespace CnabProcessor.Domain.CNAB.Services.Interfaces;

public interface ICnabParserService
{
    Task<List<Transaction>> ParseCnabFileAsync(Stream fileStream);
}