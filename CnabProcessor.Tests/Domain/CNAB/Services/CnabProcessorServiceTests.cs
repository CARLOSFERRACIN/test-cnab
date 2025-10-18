using CnabProcessor.Domain.CNAB.Services;
using CnabProcessor.Domain.CNAB.Services.Interfaces;
using CnabProcessor.Models.Entity;
using CnabProcessor.Repositories.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace CnabProcessor.Tests.Domain.CNAB.Services;

public class CnabProcessorServiceTests
{
    [Fact]
    public async Task ProcessCnabFileAsync_EmptyFile_ShouldReturnFailure()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockParserService = new Mock<ICnabParserService>();
        var mockStream = new MemoryStream();

        mockParserService.Setup(p => p.ParseCnabFileAsync(It.IsAny<Stream>()))
                        .ReturnsAsync(new List<Transaction>());

        var service = new CnabProcessorService(mockUnitOfWork.Object, mockParserService.Object);

        // Act
        var result = await service.ProcessCnabFileAsync(mockStream);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("No valid transactions found in the file.");
        mockUnitOfWork.Verify(u => u.RollbackTransactionAsync(), Times.Once);
        mockStream.Dispose();
    }

    [Fact]
    public async Task ProcessCnabFileAsync_ValidFile_ShouldProcessSuccessfully()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockParserService = new Mock<ICnabParserService>();
        var mockStoreRepository = new Mock<IStoreRepository>();
        var mockTransactionRepository = new Mock<ITransactionRepository>();
        var mockStream = new MemoryStream();

        var transactions = new List<Transaction>
        {
            new Transaction { StoreOwner = "JOÃO MACEDO", StoreName = "BAR DO JOÃO" }
        };

        mockParserService.Setup(p => p.ParseCnabFileAsync(It.IsAny<Stream>()))
                        .ReturnsAsync(transactions);
        mockUnitOfWork.Setup(u => u.Stores).Returns(mockStoreRepository.Object);
        mockUnitOfWork.Setup(u => u.Transactions).Returns(mockTransactionRepository.Object);
        mockStoreRepository.Setup(s => s.GetByOwnerAndNameAsync(It.IsAny<string>(), It.IsAny<string>()))
                          .ReturnsAsync((CnabProcessor.Models.Entity.Store?)null);
        mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
        mockTransactionRepository.Setup(t => t.BulkInsertAsync(It.IsAny<IEnumerable<Transaction>>()))
                                .Returns(Task.CompletedTask);

        var service = new CnabProcessorService(mockUnitOfWork.Object, mockParserService.Object);

        // Act
        var result = await service.ProcessCnabFileAsync(mockStream);

        // Assert
        result.Success.Should().BeTrue();
        result.ProcessedStores.Should().Be(1);
        result.ProcessedTransactions.Should().Be(1);
        mockUnitOfWork.Verify(u => u.BeginTransactionAsync(), Times.Once);
        mockUnitOfWork.Verify(u => u.CommitTransactionAsync(), Times.Once);
    }

    [Fact]
    public async Task ProcessCnabFileAsync_ExceptionOccurs_ShouldReturnFailure()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockParserService = new Mock<ICnabParserService>();
        var mockStream = new MemoryStream();

        mockParserService.Setup(p => p.ParseCnabFileAsync(It.IsAny<Stream>()))
                        .ThrowsAsync(new Exception("Parser error"));

        var service = new CnabProcessorService(mockUnitOfWork.Object, mockParserService.Object);

        // Act
        var result = await service.ProcessCnabFileAsync(mockStream);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Error processing file");
        mockUnitOfWork.Verify(u => u.RollbackTransactionAsync(), Times.Once);
        mockStream.Dispose();
    }

    [Fact]
    public async Task ProcessCnabFileAsync_ExistingStore_ShouldNotCreateNewStore()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockParserService = new Mock<ICnabParserService>();
        var mockStoreRepository = new Mock<IStoreRepository>();
        var mockTransactionRepository = new Mock<ITransactionRepository>();
        var mockStream = new MemoryStream();

        var existingStore = new CnabProcessor.Models.Entity.Store { Id = 1, Owner = "JOÃO MACEDO", Name = "BAR DO JOÃO" };
        var transactions = new List<Transaction>
        {
            new Transaction { StoreOwner = "JOÃO MACEDO", StoreName = "BAR DO JOÃO" }
        };

        mockParserService.Setup(p => p.ParseCnabFileAsync(It.IsAny<Stream>()))
                        .ReturnsAsync(transactions);
        mockUnitOfWork.Setup(u => u.Stores).Returns(mockStoreRepository.Object);
        mockUnitOfWork.Setup(u => u.Transactions).Returns(mockTransactionRepository.Object);
        mockStoreRepository.Setup(s => s.GetByOwnerAndNameAsync("JOÃO MACEDO", "BAR DO JOÃO"))
                          .ReturnsAsync(existingStore);
        mockTransactionRepository.Setup(t => t.BulkInsertAsync(It.IsAny<IEnumerable<Transaction>>()))
                                .Returns(Task.CompletedTask);

        var service = new CnabProcessorService(mockUnitOfWork.Object, mockParserService.Object);

        // Act
        var result = await service.ProcessCnabFileAsync(mockStream);

        // Assert
        result.Success.Should().BeTrue();
        result.ProcessedStores.Should().Be(0); // No new stores created
        result.ProcessedTransactions.Should().Be(1);
        mockStoreRepository.Verify(s => s.AddAsync(It.IsAny<CnabProcessor.Models.Entity.Store>()), Times.Never);
        mockStream.Dispose();
    }

    [Fact]
    public async Task ProcessCnabFileAsync_MultipleStores_ShouldProcessAllStores()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockParserService = new Mock<ICnabParserService>();
        var mockStoreRepository = new Mock<IStoreRepository>();
        var mockTransactionRepository = new Mock<ITransactionRepository>();
        var mockStream = new MemoryStream();

        var transactions = new List<Transaction>
        {
            new Transaction { StoreOwner = "JOÃO MACEDO", StoreName = "BAR DO JOÃO" },
            new Transaction { StoreOwner = "MARIA SILVA", StoreName = "LOJA DA MARIA" }
        };

        mockParserService.Setup(p => p.ParseCnabFileAsync(It.IsAny<Stream>()))
                        .ReturnsAsync(transactions);
        mockUnitOfWork.Setup(u => u.Stores).Returns(mockStoreRepository.Object);
        mockUnitOfWork.Setup(u => u.Transactions).Returns(mockTransactionRepository.Object);
        mockStoreRepository.Setup(s => s.GetByOwnerAndNameAsync(It.IsAny<string>(), It.IsAny<string>()))
                          .ReturnsAsync((CnabProcessor.Models.Entity.Store?)null);
        mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
        mockTransactionRepository.Setup(t => t.BulkInsertAsync(It.IsAny<IEnumerable<Transaction>>()))
                                .Returns(Task.CompletedTask);

        var service = new CnabProcessorService(mockUnitOfWork.Object, mockParserService.Object);

        // Act
        var result = await service.ProcessCnabFileAsync(mockStream);

        // Assert
        result.Success.Should().BeTrue();
        result.ProcessedStores.Should().Be(2);
        result.ProcessedTransactions.Should().Be(2);
        mockStoreRepository.Verify(s => s.AddAsync(It.IsAny<CnabProcessor.Models.Entity.Store>()), Times.Exactly(2));
        mockStream.Dispose();
    }
}
