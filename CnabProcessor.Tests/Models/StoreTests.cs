using CnabProcessor.Models.Entity;
using Xunit;

namespace CnabProcessor.Tests.Models;

public class StoreTests
{
    [Fact]
    public void Store_Balance_CalculatesCorrectly()
    {
        // Arrange
        var store = new Store
        {
            Id = 1,
            Owner = "JOÃO MACEDO",
            Name = "BAR DO JOÃO"
        };

        var transactions = new List<Transaction>
        {
            new Transaction { Type = 1, Amount = 100.00m }, // Debit - In
            new Transaction { Type = 2, Amount = 50.00m },  // Boleto - Out
            new Transaction { Type = 4, Amount = 25.00m },  // Credit - In
            new Transaction { Type = 9, Amount = 10.00m }    // Rent - Out
        };

        // Act
        foreach (var transaction in transactions)
        {
            store.Transactions.Add(transaction);
        }

        // Assert
        // Expected: 100 + 25 - 50 - 10 = 65
        Assert.Equal(65.00m, store.Balance);
    }

    [Fact]
    public void Store_NoTransactions_BalanceIsZero()
    {
        // Arrange
        var store = new Store
        {
            Id = 1,
            Owner = "JOÃO MACEDO",
            Name = "BAR DO JOÃO"
        };

        // Act & Assert
        Assert.Equal(0m, store.Balance);
    }

    [Fact]
    public void Store_NegativeBalance_CalculatesCorrectly()
    {
        // Arrange
        var store = new Store
        {
            Id = 1,
            Owner = "JOÃO MACEDO",
            Name = "BAR DO JOÃO"
        };

        var transactions = new List<Transaction>
        {
            new Transaction { Type = 2, Amount = 100.00m }, // Boleto - Out
            new Transaction { Type = 3, Amount = 50.00m },  // Financing - Out
            new Transaction { Type = 9, Amount = 25.00m }   // Rent - Out
        };

        // Act
        foreach (var transaction in transactions)
        {
            store.Transactions.Add(transaction);
        }

        // Assert
        // Expected: -100 - 50 - 25 = -175
        Assert.Equal(-175.00m, store.Balance);
    }
}


