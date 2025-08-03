using GadgetHub.WebAPI.Data;
using GadgetHub.WebAPI.Services;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);


// Connect to SQL Server


builder.Services.AddScoped<ProductService>(); // Change from AddSingleton to Scoped
builder.Services.AddSingleton<CustomerService>();
builder.Services.AddSingleton<QuotationStore>();
builder.Services.AddSingleton<OrderService>();
builder.Services.AddSingleton<NotificationService>();

builder.Services.AddScoped<AuthService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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

var app = builder.Build();


app.UseCors("AllowFrontend");

app.UseStaticFiles();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.Run();
