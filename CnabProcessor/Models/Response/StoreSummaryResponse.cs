namespace CnabProcessor.Models.Response;

public class StoreSummaryResponse
{
    public int Id { get; set; }
    public string Owner { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public int TransactionCount { get; set; }
}