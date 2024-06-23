﻿namespace CurrencyExchangeManager.Api.Database.DomainModels
{
    public class CurrencyHistory
    {
        public Guid Id { get; set; }
        public string Base { get; set; }
        public string Target { get; set; }
        public decimal Amount { get; set; }
    }
}