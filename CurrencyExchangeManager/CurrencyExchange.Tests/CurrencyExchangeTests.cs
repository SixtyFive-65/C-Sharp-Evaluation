using Models;
using Moq;
using Repository;

namespace CurrencyExchange.Tests
{
    [TestFixture]
    public class CurrencyExchangeTests
    {
        private Mock<ICurrencyExchangeRepository> _repo;

        public void Setup()
        {
            _repo = new Mock<ICurrencyExchangeRepository>();
        }

        [Test]
        public void Convert_ReturnConvertedAmount_ReturnAConvertedAmount()
        {
             _repo.Setup(p => p.Convert("USD", "AED", 3.673m)).ReturnsAsync(new CurrencyExchangeResponseModel { ExchangeRate = 3.673m });

            _repo.Verify(k => k.Convert("USD", "AED", 3.673m));
        }

        [Test]
        public void Convert_ReturnAndEmptyObject_ReturnEmptyOrNull()
        {
            _repo.Setup(p => p.Convert("", "", 5)).ReturnsAsync(new CurrencyExchangeResponseModel { ExchangeRate = 5 });

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