using Microsoft.AspNetCore.Mvc;
using Models;
using Repository;
using Serilog;

namespace CurrencyExchangeManager.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/")]
    public class CurrencyExchangeController : ControllerBase
    {
        private readonly ICurrencyExchangeRepository currencyExchangeRepository;

        public CurrencyExchangeController(ICurrencyExchangeRepository currencyExchangeRepository)
        {
            this.currencyExchangeRepository = currencyExchangeRepository;
        }

        [HttpGet]
        [Route("Convert")]
        public async Task<IActionResult> Convert(string @base, string target, decimal amount)
        {
            var kk = new CurrencyExchangeResponseModel();

            try
            {
                kk = await currencyExchangeRepository.Convert(@base, target, amount);
            }
            catch (Exception ex)
            {
                Log.Error($"something went wrong {ex}");

                return BadRequest(ex);
            }

            return Ok(kk);
        }

        [HttpGet]
        [Route("ConversionHistory")]
        public IActionResult ConversionHistory()
        {
            return Ok();
        }
    }
}
