﻿using CurrencyExchangeManager.Api.Database;
using CurrencyExchangeManager.Api.Models;
using CurrencyExchangeManager.Api.Repository;
using Models;
using Serilog;
using StackExchange.Redis;
using System.Text.Json;

namespace CurrencyExchangeManager.Api.Service
{
    public class CurrencyExchangeService : ICurrencyExchangeService
    {
        private readonly ICurrencyRepository currencyRepository;

        private string RedisConnectionString { get; set; }

        private string ApiUrl { get; set; }

        public CurrencyExchangeService(
            IConfiguration configuration,
            CurrencyConversionDbContext currencyConversionDbContext,
            ICurrencyRepository currencyRepository
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

                    string key = $"{@base}-{target}";

                    string value = "Hello, Redis with Interface!";

                    string retrievedValue = cacheData.StringGet(key);

                    if (!string.IsNullOrEmpty(retrievedValue))
                    {
                        conversionAmount = DoConversion(@base, target, amount, new Dictionary<string, decimal>());
                    }
                    else
                    {
                        var data = await CallRatesApi();

                        TimeSpan expiry = TimeSpan.FromMinutes(15);

                        cacheData.StringSet(key, value, expiry);

                        conversionAmount = DoConversion(@base, target, amount, data.Rates);
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

            return responsData ?? new CurrencyApiResponseModel();
        }
        public async Task<IEnumerable<CurrencyExchangeHIstoryResponseModel>> ConversionHistory()
        {
            var conversionHistory = await currencyRepository.GetCurrencyHistoryAsync();

            return conversionHistory;
        }
    }
}
