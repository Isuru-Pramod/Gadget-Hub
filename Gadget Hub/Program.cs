using GadgetHub.WebAPI.Data;
using GadgetHub.WebAPI.Services;
using Microsoft.EntityFrameworkCore;
using System.Timers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<QuotationStore>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<AuthService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure DbContext with SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// Add hosted service for background order processing
builder.Services.AddHostedService<OrderProcessingBackgroundService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthorization();
app.MapControllers();

// Initialize database and run background processing
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        await context.Database.MigrateAsync(); // Apply pending migrations

        // Initial order processing on startup
        var orderService = services.GetRequiredService<OrderService>();
        await orderService.ProcessConfirmedOrders();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while initializing the database.");
    }
}

await app.RunAsync();

// Background service for periodic order processing
public class OrderProcessingBackgroundService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<OrderProcessingBackgroundService> _logger;
    private System.Timers.Timer? _timer;

    public OrderProcessingBackgroundService(
        IServiceProvider services,
        ILogger<OrderProcessingBackgroundService> logger)
    {
        _services = services;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _timer = new System.Timers.Timer(5 * 60 * 1000); // Run every 5 minutes
        _timer.Elapsed += ProcessOrders;
        _timer.AutoReset = true;
        _timer.Enabled = true;
        return Task.CompletedTask;
    }

    private async void ProcessOrders(object? sender, ElapsedEventArgs e)
    {
        using var scope = _services.CreateScope();
        var orderService = scope.ServiceProvider.GetRequiredService<OrderService>();
        try
        {
            await orderService.ProcessConfirmedOrders();
            _logger.LogInformation("Order processing completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing orders");
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Dispose();
        await base.StopAsync(cancellationToken);
    }
}