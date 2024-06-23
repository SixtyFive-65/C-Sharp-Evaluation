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
                        Base = p.Base,
                        Target = p.Target,
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

        public Task<CurrencyHistory> SaveCurremcy(CurrencyHistory model)
        {
            throw new NotImplementedException();
        }
    }
}
