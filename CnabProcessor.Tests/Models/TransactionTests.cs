using CnabProcessor.Models.Entity;
using Xunit;

namespace CnabProcessor.Tests.Models;

public class TransactionTests
{
    [Theory]
    [InlineData(1, "Debit", "In", "+")]
    [InlineData(2, "Boleto", "Out", "-")]
    [InlineData(3, "Financing", "Out", "-")]
    [InlineData(4, "Credit", "In", "+")]
    [InlineData(5, "Loan Receipt", "In", "+")]
    [InlineData(6, "Sales", "In", "+")]
    [InlineData(7, "TED Receipt", "In", "+")]
    [InlineData(8, "DOC Receipt", "In", "+")]
    [InlineData(9, "Rent", "Out", "-")]
    public void Transaction_TypeProperties_ReturnsCorrectValues(int type, string expectedDescription, string expectedNature, string expectedSign)
    {
        // Arrange
        var transaction = new Transaction { Type = type };

        // Act & Assert
        Assert.Equal(expectedDescription, transaction.TypeDescription);
        Assert.Equal(expectedNature, transaction.Nature);
        Assert.Equal(expectedSign, transaction.Sign);
    }

    [Fact]
    public void Transaction_InvalidType_ReturnsUnknownValues()
    {
        // Arrange
        var transaction = new Transaction { Type = 99 };

        // Act & Assert
        Assert.Equal("Type 99", transaction.TypeDescription);
        Assert.Equal("Unknown", transaction.Nature);
        Assert.Equal("?", transaction.Sign);
    }
}


