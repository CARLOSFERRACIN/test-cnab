using CnabProcessor.Domain.Stores.Services;
using CnabProcessor.Models.Entity;
using CnabProcessor.Repositories.Interfaces;
using Moq;
using Xunit;

namespace CnabProcessor.Tests.Domain.Stores.Services;

public class StoreServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IStoreRepository> _mockStoreRepository;
    private readonly Mock<ITransactionRepository> _mockTransactionRepository;
    private readonly StoreService _storeService;

    public StoreServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockStoreRepository = new Mock<IStoreRepository>();
        _mockTransactionRepository = new Mock<ITransactionRepository>();

        _mockUnitOfWork.Setup(u => u.Stores).Returns(_mockStoreRepository.Object);
        _mockUnitOfWork.Setup(u => u.Transactions).Returns(_mockTransactionRepository.Object);

        _storeService = new StoreService(_mockUnitOfWork.Object);
    }

    [Fact]
    public async Task GetStoreSummariesAsync_ShouldReturnStoreSummaries()
    {
        // Arrange
        var stores = new List<Store>
        {
            new Store
            {
                Id = 1,
                Owner = "JOÃO MACEDO",
                Name = "BAR DO JOÃO",
                Transactions = new List<Transaction>
                {
                    new Transaction { Id = 1, Type = 1, Amount = 50.25m },
                    new Transaction { Id = 2, Type = 1, Amount = 50.25m }
                }
            },
            new Store
            {
                Id = 2,
                Owner = "MARIA SILVA",
                Name = "LOJA DA MARIA",
                Transactions = new List<Transaction>
                {
                    new Transaction { Id = 3, Type = 2, Amount = -200.75m }
                }
            }
        };

        _mockStoreRepository.Setup(r => r.GetStoresWithTransactionsAsync())
            .ReturnsAsync(stores);

        // Act
        var result = await _storeService.GetStoreSummariesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);

        var firstStore = result.First();
        Assert.Equal(1, firstStore.Id);
        Assert.Equal("JOÃO MACEDO", firstStore.Owner);
        Assert.Equal("BAR DO JOÃO", firstStore.Name);
        Assert.Equal(100.50m, firstStore.Balance);
        Assert.Equal(2, firstStore.TransactionCount);

        var secondStore = result.Last();
        Assert.Equal(2, secondStore.Id);
        Assert.Equal("MARIA SILVA", secondStore.Owner);
        Assert.Equal("LOJA DA MARIA", secondStore.Name);
        Assert.Equal(-200.75m, secondStore.Balance);
        Assert.Equal(1, secondStore.TransactionCount);
    }

    [Fact]
    public async Task GetStoreSummariesAsync_ShouldReturnEmptyList_WhenNoStores()
    {
        // Arrange
        _mockStoreRepository.Setup(r => r.GetStoresWithTransactionsAsync())
            .ReturnsAsync(new List<Store>());

        // Act
        var result = await _storeService.GetStoreSummariesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetStoreTransactionsAsync_ShouldReturnTransactionsForStore()
    {
        // Arrange
        var storeId = 1;
        var transactions = new List<Transaction>
        {
            new Transaction
            {
                Id = 1,
                Type = 1,
                Date = new DateTime(2023, 1, 15, 10, 30, 0),
                Amount = 100.50m,
                Cpf = "12345678901",
                Card = "123456789012",
                Time = new TimeSpan(10, 30, 0),
                StoreOwner = "JOÃO MACEDO",
                StoreName = "BAR DO JOÃO",
                StoreId = storeId
            },
            new Transaction
            {
                Id = 2,
                Type = 2,
                Date = new DateTime(2023, 1, 15, 14, 20, 0),
                Amount = 50.00m,
                Cpf = "98765432109",
                Card = "987654321098",
                Time = new TimeSpan(14, 20, 0),
                StoreOwner = "JOÃO MACEDO",
                StoreName = "BAR DO JOÃO",
                StoreId = storeId
            }
        };

        _mockTransactionRepository.Setup(r => r.GetByStoreIdAsync(storeId))
            .ReturnsAsync(transactions);

        // Act
        var result = await _storeService.GetStoreTransactionsAsync(storeId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.All(result, t => Assert.Equal(storeId, t.StoreId));
    }

    [Fact]
    public async Task GetStoreTransactionsAsync_ShouldReturnEmptyList_WhenNoTransactions()
    {
        // Arrange
        var storeId = 1;
        _mockTransactionRepository.Setup(r => r.GetByStoreIdAsync(storeId))
            .ReturnsAsync(new List<Transaction>());

        // Act
        var result = await _storeService.GetStoreTransactionsAsync(storeId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetStoreTransactionsAsync_ShouldThrowException_WhenRepositoryFails()
    {
        // Arrange
        var storeId = 1;
        _mockTransactionRepository.Setup(r => r.GetByStoreIdAsync(storeId))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _storeService.GetStoreTransactionsAsync(storeId));
    }

    [Fact]
    public async Task GetStoreSummariesAsync_ShouldThrowException_WhenRepositoryFails()
    {
        // Arrange
        _mockStoreRepository.Setup(r => r.GetStoresWithTransactionsAsync())
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _storeService.GetStoreSummariesAsync());
    }
}
