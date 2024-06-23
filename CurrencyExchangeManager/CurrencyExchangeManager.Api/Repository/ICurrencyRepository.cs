using CurrencyExchangeManager.Api.Database.DomainModels;
using Models;

namespace CurrencyExchangeManager.Api.Repository
{
    public interface ICurrencyRepository
    {
        Task<IEnumerable<CurrencyExchangeHIstoryResponseModel>> GetCurrencyHistoryAsync();

        Task<bool> SaveRatesAsync(Dictionary<string, decimal> rates);
    }
}
