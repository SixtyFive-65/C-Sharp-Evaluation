using CurrencyExchangeManager.Api.Models;
using Models;

namespace CurrencyExchangeManager.Api.Service
{
    public interface ICurrencyExchangeService
    {
        Task<CurrencyExchangeResponseModel> Convert(string @base, string target, decimal amount);
        Task<CurrencyApiResponseModel> CallRatesApi();
        Task<IEnumerable<CurrencyExchangeHIstoryResponseModel>> ConversionHistory();
    }
}
