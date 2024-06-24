using Castle.Core.Logging;
using CurrencyExchangeManager.Api.Service;
using Microsoft.Extensions.Configuration;
using Models;
using Moq;
using StackExchange.Redis;
using System.Text.Json;

namespace CurrencyExchange.Tests
{
    [TestFixture]
    public class CurrencyExchangeTests
    {
        private Mock<IRedisHelper> redisHelperMock;
        private Mock<IDatabase> databaseMock;

        private CurrencyExchangeService currencyExchangeService;

        public string BaseCurrency { get; set; }
        public string TargetCurrency { get; set; }
        public decimal Amount { get; set; }
        public Dictionary<string, decimal> CachedRates { get; set; }

        private readonly IConfiguration configuration;

        public void Setup()
        {
            redisHelperMock = new Mock<IRedisHelper>();
            databaseMock = new Mock<IDatabase>();

            currencyExchangeService = new CurrencyExchangeService(configuration, null, null, redisHelperMock.Object);

            BaseCurrency = "USD";
            TargetCurrency = "USD";
            Amount = 10.0m;

            CachedRates = new Dictionary<string, decimal>
                           {
                               { "EUR", 0.85m },
                               { "GBP", 0.78m }
                           };

            redisHelperMock.Setup(r => r.GetDatabase()).Returns(databaseMock.Object);
        }

        [Test]
        [Ignore("Coulnd't complete")]
        public async Task Convert_ReturnConvertedAmount_ReturnAConvertedAmount()
        {
            try
            {
                var cachedValue = JsonSerializer.Serialize(CachedRates);

                 databaseMock.Setup(d => d.StringGet("CurrencyRates")).Returns(new RedisValue(cachedValue));

                var result = await currencyExchangeService.Convert(BaseCurrency, TargetCurrency, Amount);

                Assert.IsNotNull(result, "Result should not be null");
            }
            catch (Exception ex)
            {
                Assert.Fail($"Test failed with exception: {ex}");
            }
        }

        [Test]
        [Ignore("Coulnd't complete")]
        public async Task Convert_ThrowExceptionIfTheresARunTimeError_DontThrowException()
        {
            try
            {
                var expectedErrorMessage = "API call failed";

                // databaseMock.Setup(d => d.StringGet("CurrencyRates")).Returns((string)null);

                var mockApiService = new Mock<ICurrencyExchangeService>();
                mockApiService.Setup(a => a.CallRatesApi()).ThrowsAsync(new Exception(expectedErrorMessage));

                var result = await currencyExchangeService.Convert(BaseCurrency, TargetCurrency, Amount);

                Assert.AreEqual(0, result.ConvertedAmount); // Check if converted amount is zero on failure
            }
            catch (Exception ex)
            {
                Assert.Fail($"Test failed with exception: {ex}");
            }
        }
    }
}