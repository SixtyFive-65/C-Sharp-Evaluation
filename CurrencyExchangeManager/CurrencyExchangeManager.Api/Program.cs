using CurrencyExchangeManager.Api.Database;
using Microsoft.EntityFrameworkCore;
using Serilog;
using CurrencyExchangeManager.Api.Repository;
using CurrencyExchangeManager.Api.Service;

var builder = WebApplication.CreateBuilder(args);

var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("C:\\CurrencyExchangeLogs/logs.txt", rollingInterval: RollingInterval.Day)
    .MinimumLevel.Warning()
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);


// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<CurrencyConversionDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("CurrencyConversion"));
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ICurrencyExchangeService, CurrencyExchangeService>();

builder.Services.AddScoped<ICurrencyRepository, CurrencyRepository>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
