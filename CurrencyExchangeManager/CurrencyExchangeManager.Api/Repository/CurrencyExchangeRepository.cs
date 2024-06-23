using CurrencyExchangeManager.Api.Database;
using CurrencyExchangeManager.Api.Models;
using Microsoft.EntityFrameworkCore;
using Models;
using Serilog;
using StackExchange.Redis;
using System.Text.Json;

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
            var conversionAmount = 0.00m;

            try
            {
                using (var redisConnector = new RedisHelper(redisConnectionString))
                {
                    IDatabase cacheData = redisConnector.GetDatabase();

                    string key = $"{@base}-{target}";

                    string value = "Hello, Redis with Interface!";

                    string retrievedValue = cacheData.StringGet(key);

                    if (!string.IsNullOrEmpty(retrievedValue))
                    {
                        conversionAmount =  DoConversion(@base, target, amount, new Dictionary<string, decimal>() );
                    }
                    else
                    {
                        var data = await CallRatesApi();

                        TimeSpan expiry = TimeSpan.FromMinutes(15);

                        cacheData.StringSet(key, value, expiry);

                        conversionAmount =  DoConversion(@base, target, amount, data.Rates );
                    }

                    Console.WriteLine($"Value retrieved from Redis: {retrievedValue}");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"{ex?.InnerException?.Message}");

                return new CurrencyExchangeResponseModel { ConvertedAmount = 0, ResponseMessage = ex.Message };
            }

            return new CurrencyExchangeResponseModel() { ConvertedAmount = conversionAmount, ResponseMessage = "Success" };
        }

        private decimal DoConversion(string baseCurrency, string targetCurrency, decimal amount, Dictionary<string,decimal> rates)
        {
            decimal amountInTargetCurrency = 0.00m;

            if (rates.TryGetValue(targetCurrency, out decimal exchangeRate))
            {
                amountInTargetCurrency = amount * exchangeRate;

                Console.WriteLine($"Converted {amount} {baseCurrency} to {targetCurrency}: {amountInTargetCurrency}");
            }
            else
            {
                Log.Warning($"Cannot convert {amount} from {baseCurrency} to {targetCurrency}");

                Console.WriteLine($"Exchange rate for {targetCurrency} not found.");
            }

            return amountInTargetCurrency;
        }

        private async Task<CurrencyApiResponseModel> CallRatesApi()
        {
            var responsData = new CurrencyApiResponseModel();

            using (var client = new HttpClient())
            {
                var response = await client.GetAsync("https://openexchangerates.org/api/latest.json?app_id=b752eef05538469488b72c19acbfa918");

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(jsonResponse);

                    responsData = JsonSerializer.Deserialize<CurrencyApiResponseModel>(@jsonResponse);
                }
                else
                {
                    Log.Error($"Erro calling {response.RequestMessage} - {response.StatusCode}");
                    Console.WriteLine($"Failed to retrieve data. Status code: {response.StatusCode}");

                    return new CurrencyApiResponseModel() { ResponseMessage = $"Failed to call {response.RequestMessage} - {response.StatusCode}" };
                }
            }

            return responsData ?? new CurrencyApiResponseModel();
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

                response.Add(new CurrencyExchangeHIstoryResponseModel { ResponseMessage = ex.Message });

                return response;
            }

            return response;
        }
    }
}
