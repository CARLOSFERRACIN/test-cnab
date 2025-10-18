using CnabProcessor.Domain.CNAB.Services;
using CnabProcessor.Domain.CNAB.Services.Interfaces;
using CnabProcessor.Repositories.Data;
using CnabProcessor.Repositories.Interfaces;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CnabProcessor.Tests;

public class ProgramTests
{
    [Fact]
    public void Program_ShouldRegisterAllServices()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddDbContext<CnabContext>(options => options.UseSqlite("DataSource=:memory:"));
        services.AddScoped<IStoreRepository, CnabProcessor.Repositories.StoreRepository>();
        services.AddScoped<ITransactionRepository, CnabProcessor.Repositories.TransactionRepository>();
        services.AddScoped<IUnitOfWork, CnabProcessor.Repositories.UnitOfWork>();
        services.AddScoped<ICnabParserService, CnabParserService>();
        services.AddScoped<ICnabProcessorService, CnabProcessorService>();

        // Act
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        serviceProvider.GetService<IStoreRepository>().Should().NotBeNull();
        serviceProvider.GetService<ITransactionRepository>().Should().NotBeNull();
        serviceProvider.GetService<IUnitOfWork>().Should().NotBeNull();
        serviceProvider.GetService<ICnabParserService>().Should().NotBeNull();
        serviceProvider.GetService<ICnabProcessorService>().Should().NotBeNull();
    }
}
