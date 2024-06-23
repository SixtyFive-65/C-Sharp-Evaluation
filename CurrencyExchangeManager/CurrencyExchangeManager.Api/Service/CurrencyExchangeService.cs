using Azure;
using CurrencyExchangeManager.Api.Database;
using CurrencyExchangeManager.Api.Models;
using CurrencyExchangeManager.Api.Repository;
using Models;
using Serilog;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CurrencyExchangeManager.Api.Service
{
    public class CurrencyExchangeService : ICurrencyExchangeService
    {
        private readonly ICurrencyRepository currencyRepository;

        private string RedisConnectionString { get; set; }

        private string ApiUrl { get; set; }

        public CurrencyExchangeService(
            IConfiguration configuration = null,
            CurrencyConversionDbContext currencyConversionDbContext = null,
            ICurrencyRepository currencyRepository = null
            )
        {
            RedisConnectionString = configuration?.GetSection("RedisServer")?.Value?.ToString();
            ApiUrl = configuration?.GetSection("CurrencyApiUrl")?.Value?.ToString();
            this.currencyRepository = currencyRepository;
        }

        public async Task<CurrencyExchangeResponseModel> Convert(string @base, string target, decimal amount)
        {
            var conversionAmount = new CurrencyExchangeResponseModel();

            try
            {
                using (var redisConnector = new RedisHelper(RedisConnectionString))
                {
                    IDatabase cacheData = redisConnector.GetDatabase();

                    string key = $"CurrencyRates";

                    string retrievedValue = cacheData.StringGet(key);

                    if (!string.IsNullOrEmpty(retrievedValue))
                    {
                        conversionAmount = DoConversion(@base, target, amount, JsonSerializer.Deserialize<Dictionary<string,decimal>>(retrievedValue));
                    }
                    else
                    {
                        var data = await CallRatesApi();

                        TimeSpan expiry = TimeSpan.FromMinutes(15);

                        conversionAmount = DoConversion(@base, target, amount, data.Rates);  // we should store this rate

                        string json = JsonSerializer.Serialize(data.Rates);

                        cacheData.StringSet(key, json, expiry);


                        bool saveRates = await SaveRates(data.Rates);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"{ex?.InnerException?.Message}");

                return new CurrencyExchangeResponseModel { ConvertedAmount = 0, ResponseMessage = ex.Message };
            }

            return conversionAmount;
        }

        private CurrencyExchangeResponseModel DoConversion(string baseCurrency, string targetCurrency, decimal amount, Dictionary<string, decimal> rates)
        {
            var amountInTargetCurrency = new CurrencyExchangeResponseModel();

            if (rates.TryGetValue(targetCurrency, out decimal exchangeRate))
            {
                amountInTargetCurrency.ConvertedAmount = amount * exchangeRate;
                amountInTargetCurrency.ResponseMessage = "Success";

                Console.WriteLine($"Converted {amount} {baseCurrency} to {targetCurrency}: {amountInTargetCurrency}");
            }
            else
            {
                amountInTargetCurrency.ResponseMessage = $"Exchange rate for {targetCurrency} not found.";

                Log.Warning($"Cannot convert {amount} from {baseCurrency} to {targetCurrency}");

                Console.WriteLine($"Exchange rate for {targetCurrency} not found.");
            }

            return amountInTargetCurrency;
        }

        private async Task<CurrencyApiResponseModel> CallRatesApi()
        {
            var responsData = new CurrencyApiResponseModel();

            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(ApiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResponse = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(jsonResponse);

                        responsData = JsonSerializer.Deserialize<CurrencyApiResponseModel>(@jsonResponse);
                    }
                    else
                    {
                        Log.Error($"Failed to retrieve data {response.RequestMessage} - {response.StatusCode}");
                        Console.WriteLine($"Failed to retrieve data. Status code: {response.StatusCode}");

                        return new CurrencyApiResponseModel() { ResponseMessage = $"Failed to retrieve data {response.RequestMessage} - {response.StatusCode}" };
                    }
                }
            }
            catch(Exception e)
            {
                Log.Error($"Failed to retrieve data {e.Message}");

                responsData.ResponseMessage = e.Message;

                return responsData;
            }

            return responsData;
        }
        public async Task<IEnumerable<CurrencyExchangeHIstoryResponseModel>> ConversionHistory()
        {
            var conversionHistory = await currencyRepository.GetCurrencyHistoryAsync();

            return conversionHistory;
        }

        public async Task<bool> SaveRates(Dictionary<string, decimal> rates)
        {
            var conversionHistory = await currencyRepository.SaveRatesAsync(rates);

            return conversionHistory;
        }
    }
}
