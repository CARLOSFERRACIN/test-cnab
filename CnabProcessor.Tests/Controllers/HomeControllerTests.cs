using CnabProcessor.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace CnabProcessor.Tests.Controllers;

public class HomeControllerTests
{
    [Fact]
    public void Index_ShouldReturnView()
    {
        // Arrange
        var controller = new HomeController();

        // Act
        var result = controller.Index();

        // Assert
        result.Should().BeOfType<ViewResult>();
        var viewResult = result as ViewResult;
        viewResult!.ViewName.Should().BeNull(); // Should return default view (Index)
    }
}
