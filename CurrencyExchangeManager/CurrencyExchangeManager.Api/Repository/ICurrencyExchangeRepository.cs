using Models;

namespace Repository
{
    public interface ICurrencyExchangeRepository
    {
        Task<CurrencyExchangeResponseModel> Convert(string @base, string target, decimal amount);
        Task<IEnumerable<CurrencyExchangeHIstoryResponseModel>> ConversionHistory();
    }
}
