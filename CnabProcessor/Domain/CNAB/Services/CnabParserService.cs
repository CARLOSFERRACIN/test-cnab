using CnabProcessor.Domain.CNAB.Services.Interfaces;
using CnabProcessor.Models.Entity;

namespace CnabProcessor.Domain.CNAB.Services;

public class CnabParserService : ICnabParserService
{
    public async Task<List<Transaction>> ParseCnabFileAsync(Stream fileStream)
    {
        var transactions = new List<Transaction>();
        
        using var reader = new StreamReader(fileStream);
        string? line;
        
        while ((line = await reader.ReadLineAsync()) != null)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;
                
            var transaction = ParseCnabLine(line);
            if (transaction != null)
            {
                transactions.Add(transaction);
            }
        }
        
        return transactions;
    }
    
    private static Transaction? ParseCnabLine(string line)
    {
        if (line.Length < 80)
            return null;
            
        try
        {
            var type = int.Parse(line.Substring(0, 1));
            var dateStr = line.Substring(1, 8);
            var amountStr = line.Substring(9, 10);
            var cpf = line.Substring(19, 11);
            var card = line.Substring(30, 12);
            var timeStr = line.Substring(42, 6);
            var storeOwner = line.Substring(48, 14).Trim();
            var storeName = line.Substring(62).Trim();
            
            var year = int.Parse(dateStr.Substring(0, 4));
            var month = int.Parse(dateStr.Substring(4, 2));
            var day = int.Parse(dateStr.Substring(6, 2));
            var date = new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc);
            
            var amount = decimal.Parse(amountStr) / 100m;
            
            var hour = int.Parse(timeStr.Substring(0, 2));
            var minute = int.Parse(timeStr.Substring(2, 2));
            var second = int.Parse(timeStr.Substring(4, 2));
            var time = new TimeSpan(hour, minute, second);
            
            return new Transaction
            {
                Type = type,
                Date = date,
                Amount = amount,
                Cpf = cpf,
                Card = card,
                Time = time,
                StoreOwner = storeOwner,
                StoreName = storeName
            };
        }
        catch
        {
            return null;
        }
    }
}