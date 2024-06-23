using System.Text.Json.Serialization;

namespace CurrencyExchangeManager.Api.Models
{
    public class CurrencyApiResponseModel
    {
        [JsonPropertyName("disclaimer")]
        public string Disclaimer { get; set; }
       
        [JsonPropertyName("license")]
        public  string License { get; set; }
       
        [JsonPropertyName("timeStamp")]
        public DateTime TimeStamp { get; set; }
      
        [JsonPropertyName("base")]
        public  string Base { get; set; }

        [JsonPropertyName("rates")]
        public Dictionary<string, decimal> Rates { get; set; }

        public  string ResponseMessage { get; set; }
    }
}
