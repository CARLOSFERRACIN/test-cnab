using CnabProcessor.Domain.CNAB.Services;
using Xunit;

namespace CnabProcessor.Tests.Domain.CNAB.Services;

public class CnabParserServiceTests
{
    private readonly CnabParserService _parserService;

    public CnabParserServiceTests()
    {
        _parserService = new CnabParserService();
    }

    [Fact]
    public async Task ParseCnabFileAsync_RealFile_ReturnsTransactions()
    {
        // Arrange
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "CNAB.txt");
        using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

        // Act
        var result = await _parserService.ParseCnabFileAsync(stream);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Count > 0);

        // Check first transaction (from real file)
        var firstTransaction = result[0];
        Assert.Equal(3, firstTransaction.Type);
        // Date now includes time component and is converted from UTC-3 to UTC
        // Original: 2019-03-01 15:34:53 UTC-3 -> 2019-03-01 18:34:53 UTC
        Assert.Equal(new DateTime(2019, 3, 1, 18, 34, 53, DateTimeKind.Utc), firstTransaction.Date);
        Assert.Equal(142.00m, firstTransaction.Amount);
        Assert.Equal("09620676017", firstTransaction.Cpf);
        Assert.Equal("4753****3153", firstTransaction.Card);
        Assert.Equal("JOÃO MACEDO", firstTransaction.StoreOwner.Trim());
        Assert.Equal("BAR DO JOÃO", firstTransaction.StoreName.Trim());
    }

    [Fact]
    public async Task ParseCnabFileAsync_EmptyFile_ReturnsEmptyList()
    {
        // Arrange
        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(""));

        // Act
        var result = await _parserService.ParseCnabFileAsync(stream);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task ParseCnabFileAsync_InvalidLine_SkipsInvalidLine()
    {
        // Arrange - Using two valid CNAB lines from the real file
        var cnabContent = "invalid line\n" +
                         "5201903010000013200556418150633123****7687145607MARIA JOSEFINALOJA DO Ó - MATRIZ\n";

        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(cnabContent));

        // Act
        var result = await _parserService.ParseCnabFileAsync(stream);

        // Debug output
        Console.WriteLine($"Number of transactions parsed: {result.Count}");
        foreach (var transaction in result)
        {
            Console.WriteLine($"Transaction: Type={transaction.Type}, Amount={transaction.Amount}, Store={transaction.StoreName}");
        }

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Count); // Should skip invalid line
    }

    [Fact]
    public async Task ParseCnabFileAsync_RealFile_ProcessesAllTransactions()
    {
        // Arrange
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "CNAB.txt");
        using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

        // Act
        var result = await _parserService.ParseCnabFileAsync(stream);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(21, result.Count); // Real file has 21 transactions

        // Verify all transactions have valid data
        foreach (var transaction in result)
        {
            Assert.True(transaction.Type > 0);
            Assert.True(transaction.Amount > 0);
            Assert.False(string.IsNullOrEmpty(transaction.Cpf));
            Assert.False(string.IsNullOrEmpty(transaction.Card));
            Assert.False(string.IsNullOrEmpty(transaction.StoreOwner));
            Assert.False(string.IsNullOrEmpty(transaction.StoreName));
        }
    }
}


