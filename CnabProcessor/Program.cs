using CnabProcessor.Domain.CNAB.Services;
using CnabProcessor.Domain.CNAB.Services.Interfaces;
using CnabProcessor.Domain.Stores.Services;
using CnabProcessor.Domain.Stores.Services.Interfaces;
using CnabProcessor.Repositories;
using CnabProcessor.Repositories.Data;
using CnabProcessor.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "CNAB Processor API",
        Version = "v1",
        Description = "API for processing CNAB files and managing financial transactions"
    });

    // Include XML comments
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

builder.Services.AddDbContext<CnabContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories
builder.Services.AddScoped<IStoreRepository, StoreRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Services
builder.Services.AddScoped<ICnabParserService, CnabParserService>();
builder.Services.AddScoped<ICnabProcessorService, CnabProcessorService>();
builder.Services.AddScoped<IStoreService, StoreService>();

var app = builder.Build();

// Configure Swagger based on environment
var enableSwagger = app.Configuration.GetValue<bool>("EnableSwagger");
if (enableSwagger)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Log environment information
var environment = app.Configuration.GetValue<string>("Environment");
var detailedErrors = app.Configuration.GetValue<bool>("DetailedErrors");
Console.WriteLine($"Environment: {environment}");
Console.WriteLine($"Detailed Errors: {detailedErrors}");
Console.WriteLine($"Swagger Enabled: {enableSwagger}");

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Apply migrations before starting the application
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<CnabContext>();
    try
    {
        Console.WriteLine("Applying database migrations...");
        context.Database.Migrate();
        Console.WriteLine("Migrations applied successfully");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error applying migrations: {ex.Message}");
        try
        {
            Console.WriteLine("Attempting to create database...");
            context.Database.EnsureCreated();
            Console.WriteLine("Database created successfully");
        }
        catch (Exception ex2)
        {
            Console.WriteLine($"Error creating database: {ex2.Message}");
            throw; // Re-throw to prevent application from starting with broken database
        }
    }
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers();

app.Run();
