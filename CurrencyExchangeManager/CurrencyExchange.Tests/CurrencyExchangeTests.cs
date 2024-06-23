using CurrencyExchangeManager.Api.Service;
using Models;
using Moq;

namespace CurrencyExchange.Tests
{
    [TestFixture]
    public class CurrencyExchangeTests
    {
        private Mock<ICurrencyExchangeService> _repo;

        public void Setup()
        {
            _repo = new Mock<ICurrencyExchangeService>();
        }

        [Test]
        public void Convert_ReturnConvertedAmount_ReturnAConvertedAmount()
        {
             _repo.Setup(p => p.Convert("USD", "AED", 3.673m)).ReturnsAsync(new CurrencyExchangeResponseModel { ConvertedAmount = 3.673m, ResponseMessage = "Success" });

            _repo.Verify(k => k.Convert("USD", "AED", 3.673m));
        }

        [Test]
        public void Convert_ReturnAndEmptyObject_ReturnEmptyOrNull()
        {
            _repo.Setup(p => p.Convert("", "", 5)).ReturnsAsync(new CurrencyExchangeResponseModel { ConvertedAmount = 0 });

            _repo.Verify(k => k.Convert("", "", 5));
        }

        [Test]
        public void Convert_ThrowExceptionIfTheresARunTimeError_DontThrowException()
        {
            _repo.Setup(p => p.Convert("", "", 3.673m)).Throws<Exception>();

            _repo.Verify(k => k.Convert("", "", 5));
        }
    }
}