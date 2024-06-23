using Azure;
using CurrencyExchangeManager.Api.Database;
using CurrencyExchangeManager.Api.Database.DomainModels;
using Microsoft.EntityFrameworkCore;
using Models;
using Serilog;

namespace CurrencyExchangeManager.Api.Repository
{
    public class CurrencyRepository : ICurrencyRepository
    {
        private readonly CurrencyConversionDbContext currencyConversionDbContext;

        public CurrencyRepository(CurrencyConversionDbContext currencyConversionDbContext)
        {
            this.currencyConversionDbContext = currencyConversionDbContext;
        }
        public async Task<IEnumerable<CurrencyExchangeHIstoryResponseModel>> GetCurrencyHistoryAsync()
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
                        ExchangeRate = p.ExchangeRate,
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

        public async Task<bool> SaveRatesAsync(Dictionary<string, decimal> rates)
        {
            int saveResult = 0;
            try
            {
                var saveCurrencyModel = rates.Select(p => new CurrencyHistory()
                {
                    ExchangeRate = p.Key,
                    Amount = p.Value,
                    EditedDateTime = DateTime.UtcNow,

                }).ToList();

                await currencyConversionDbContext.AddRangeAsync(saveCurrencyModel);

                saveResult = await currencyConversionDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.Error($"{ex?.InnerException?.Message}");

                throw ex;
            }

            if(saveResult > 0)
            {
                return true;
            }

           return false;
        }
    }
}
