namespace CurrencyExchangeManager.Api.Database.DomainModels
{
    public class CurrencyHistory
    {
        public Guid Id { get; set; }
        public string ExchangeRate { get; set; }
        public decimal Amount { get; set; }
        public DateTime EditedDateTime { get; set; }
        public int Version { get; set; }
        public bool IsDeleted { get; set; }
    }
}
