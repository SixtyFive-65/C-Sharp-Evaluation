using Models;

namespace CurrencyExchangeManager.Api.Service
{
    public interface ICurrencyExchangeService
    {
        Task<CurrencyExchangeResponseModel> Convert(string @base, string target, decimal amount);
        Task<IEnumerable<CurrencyExchangeHIstoryResponseModel>> ConversionHistory();
    }
}
