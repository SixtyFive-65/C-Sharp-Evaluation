namespace Models
{
    public class CurrencyExchangeHIstoryResponseModel
    {
        public Guid Id { get; set; }
        public string ExchangeRate { get; set; }
        public decimal Amount { get; set; }
        public string ResponseMessage { get; set; }
    }
}
