using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class CurrencyExchangeRequestModel
    {
        [Required(ErrorMessage = "Base is required!")]
        public string Base { get; set; }
        [Required(ErrorMessage = "Target is required!")]
        public string Target { get; set; }
        [Required(ErrorMessage = "Amount is required and Should be numeric!")]
        [Range(0, 999.99)]
        public decimal Amount { get; set; }
    }
}
