using CnabProcessor.Controllers;
using CnabProcessor.Domain.Stores.Services.Interfaces;
using CnabProcessor.Models.Entity;
using CnabProcessor.Models.Response;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CnabProcessor.Tests.Controllers;

public class StoresControllerTests
{
    [Fact]
    public async Task GetStores_ShouldReturnOk()
    {
        // Arrange
        var mockStoreService = new Mock<IStoreService>();
        var mockLogger = new Mock<ILogger<StoresController>>();
        var controller = new StoresController(mockStoreService.Object, mockLogger.Object);

        var expectedStores = new List<StoreSummaryResponse>
        {
            new StoreSummaryResponse { Id = 1, Owner = "JOÃO MACEDO", Name = "BAR DO JOÃO", Balance = 100.50m, TransactionCount = 5 }
        };
        mockStoreService.Setup(s => s.GetStoreSummariesAsync())
                       .ReturnsAsync(expectedStores);

        // Act
        var result = await controller.GetStores();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(expectedStores);
    }

    [Fact]
    public async Task GetStores_ServiceThrowsException_ShouldReturnInternalServerError()
    {
        // Arrange
        var mockStoreService = new Mock<IStoreService>();
        var mockLogger = new Mock<ILogger<StoresController>>();
        var controller = new StoresController(mockStoreService.Object, mockLogger.Object);

        mockStoreService.Setup(s => s.GetStoreSummariesAsync())
                       .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await controller.GetStores();

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = result as ObjectResult;
        objectResult!.StatusCode.Should().Be(500);
        objectResult.Value.Should().Be("Internal server error while retrieving stores.");
    }

    [Fact]
    public async Task GetStoreTransactions_ValidStoreId_ShouldReturnOk()
    {
        // Arrange
        var mockStoreService = new Mock<IStoreService>();
        var mockLogger = new Mock<ILogger<StoresController>>();
        var controller = new StoresController(mockStoreService.Object, mockLogger.Object);

        var storeId = 1;
        var expectedTransactions = new List<Transaction>
        {
            new Transaction
            {
                Id = 1,
                Type = 1,
                Date = DateTime.UtcNow,
                Amount = 100.50m,
                Cpf = "12345678901",
                Card = "123456789012",
                StoreOwner = "JOÃO MACEDO",
                StoreName = "BAR DO JOÃO",
                StoreId = storeId
            }
        };

        mockStoreService.Setup(s => s.GetStoreTransactionsAsync(storeId))
                       .ReturnsAsync(expectedTransactions);

        // Act
        var result = await controller.GetStoreTransactions(storeId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(expectedTransactions);
    }

    [Fact]
    public async Task GetStoreTransactions_ServiceThrowsException_ShouldReturnInternalServerError()
    {
        // Arrange
        var mockStoreService = new Mock<IStoreService>();
        var mockLogger = new Mock<ILogger<StoresController>>();
        var controller = new StoresController(mockStoreService.Object, mockLogger.Object);

        var storeId = 1;
        mockStoreService.Setup(s => s.GetStoreTransactionsAsync(storeId))
                       .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await controller.GetStoreTransactions(storeId);

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = result as ObjectResult;
        objectResult!.StatusCode.Should().Be(500);
        objectResult.Value.Should().Be("Internal server error while retrieving store transactions.");
    }

    [Fact]
    public async Task GetStoreTransactions_EmptyResult_ShouldReturnOkWithEmptyList()
    {
        // Arrange
        var mockStoreService = new Mock<IStoreService>();
        var mockLogger = new Mock<ILogger<StoresController>>();
        var controller = new StoresController(mockStoreService.Object, mockLogger.Object);

        var storeId = 999;
        var expectedTransactions = new List<Transaction>();

        mockStoreService.Setup(s => s.GetStoreTransactionsAsync(storeId))
                       .ReturnsAsync(expectedTransactions);

        // Act
        var result = await controller.GetStoreTransactions(storeId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(expectedTransactions);
    }
}
