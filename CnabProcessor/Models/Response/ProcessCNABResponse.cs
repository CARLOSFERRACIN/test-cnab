namespace CnabProcessor.Models.Response;

public class ProcessCNABResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int ProcessedStores { get; set; }
    public int ProcessedTransactions { get; set; }
}
