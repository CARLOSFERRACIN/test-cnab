using CnabProcessor.Controllers;
using CnabProcessor.Domain.CNAB.Services.Interfaces;
using CnabProcessor.Models.Response;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CnabProcessor.Tests.Controllers;

public class CnabControllerTests
{
    [Fact]
    public async Task UploadCnabFile_NoFile_ShouldReturnBadRequest()
    {
        // Arrange
        var mockProcessorService = new Mock<ICnabProcessorService>();
        var mockLogger = new Mock<ILogger<CnabController>>();
        var controller = new CnabController(mockProcessorService.Object, mockLogger.Object);

        // Act
        var result = await controller.UploadCnabFile(null);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().Be("No file was uploaded.");
    }

    [Fact]
    public async Task UploadCnabFile_InvalidFileType_ShouldReturnBadRequest()
    {
        // Arrange
        var mockProcessorService = new Mock<ICnabProcessorService>();
        var mockLogger = new Mock<ILogger<CnabController>>();
        var controller = new CnabController(mockProcessorService.Object, mockLogger.Object);
        
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.FileName).Returns("test.pdf");
        mockFile.Setup(f => f.Length).Returns(100);

        // Act
        var result = await controller.UploadCnabFile(mockFile.Object);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().Be("The file must be a text file (.txt).");
    }

    [Fact]
    public async Task UploadCnabFile_ValidFile_ShouldReturnOk()
    {
        // Arrange
        var mockProcessorService = new Mock<ICnabProcessorService>();
        var mockLogger = new Mock<ILogger<CnabController>>();
        var controller = new CnabController(mockProcessorService.Object, mockLogger.Object);
        
        var mockFile = new Mock<IFormFile>();
        var mockStream = new MemoryStream();
        mockFile.Setup(f => f.FileName).Returns("test.txt");
        mockFile.Setup(f => f.Length).Returns(100);
        mockFile.Setup(f => f.OpenReadStream()).Returns(mockStream);
        
        var expectedResult = new ProcessCNABResponse { Success = true, Message = "Success" };
        mockProcessorService.Setup(p => p.ProcessCnabFileAsync(It.IsAny<Stream>()))
                           .ReturnsAsync(expectedResult);

        // Act
        var result = await controller.UploadCnabFile(mockFile.Object);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().Be(expectedResult);
        mockStream.Dispose();
    }

}
