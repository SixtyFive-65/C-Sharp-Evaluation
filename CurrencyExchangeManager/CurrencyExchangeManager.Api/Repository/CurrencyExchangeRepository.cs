using CurrencyExchangeManager.Api.Database;
using Microsoft.EntityFrameworkCore;
using Models;
using Serilog;
using StackExchange.Redis;

namespace Repository
{
    public class CurrencyExchangeRepository : ICurrencyExchangeRepository
    {
        private readonly CurrencyConversionDbContext currencyConversionDbContext;

        private string redisConnectionString { get; set; }

        public CurrencyExchangeRepository(IConfiguration configuration, CurrencyConversionDbContext currencyConversionDbContext)
        {
            redisConnectionString = configuration?.GetSection("RedisServer")?.Value?.ToString();
            this.currencyConversionDbContext = currencyConversionDbContext;
        }

        public async Task<CurrencyExchangeResponseModel> Convert(string @base, string target, decimal amount)
        {
            try
            {
                using (var redisConnector = new RedisHelper(redisConnectionString))
                {
                    IDatabase db = redisConnector.GetDatabase();

                    string key = $"{@base}-{target}-{amount}";

                    string value = "Hello, Redis with Interface!";
                 
                    // Example: Get a value by key
                    string retrievedValue = db.StringGet(key);

                    if (!string.IsNullOrEmpty(retrievedValue))
                    {
                        new CurrencyExchangeResponseModel() { ExchangeRate = 10 };
                    }
                    else
                    {
                        //retreive from API 

                        db.StringSet(key, value);

                        return new CurrencyExchangeResponseModel() { ExchangeRate = 25 };
                    }
                    Console.WriteLine($"Value retrieved from Redis: {retrievedValue}");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"{ex?.InnerException?.Message}");
            }

            return new CurrencyExchangeResponseModel() { ExchangeRate = 5};
        }

        public async Task<IEnumerable<CurrencyExchangeHIstoryResponseModel>> ConversionHistory()
        {
            var response = new List<CurrencyExchangeHIstoryResponseModel>();

            try
            {
                var data = await currencyConversionDbContext.CurrencyHistory.ToListAsync();

                if (data != null)
                {
                    response = data.Select(p => new CurrencyExchangeHIstoryResponseModel
                    {
                        Id = p.Id,
                        Base = p.Base,
                        Target = p.Target,
                        Amount = p.Amount
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                Log.Error($"{ex?.InnerException?.Message}");
            }

            return response;
        }
    }
}
