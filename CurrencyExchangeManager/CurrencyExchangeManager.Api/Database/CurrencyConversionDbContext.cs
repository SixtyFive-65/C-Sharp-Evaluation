using CurrencyExchangeManager.Api.Database.DomainModels;
using Microsoft.EntityFrameworkCore;

namespace CurrencyExchangeManager.Api.Database
{
    public class CurrencyConversionDbContext : DbContext
    {
        public CurrencyConversionDbContext(DbContextOptions options) : base(options)
        {

           
        }

        public DbSet<CurrencyHistory> CurrencyHistory { get; set; }
    }
}
