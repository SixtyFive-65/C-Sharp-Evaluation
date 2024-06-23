using CurrencyExchangeManager.Api.Service;
using Microsoft.AspNetCore.Mvc;
using Models;
using Serilog;


namespace CurrencyExchangeManager.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/")]
    public class CurrencyExchangeController : ControllerBase
    {
        private readonly ICurrencyExchangeService currencyExchangeRepository;

        public CurrencyExchangeController(ICurrencyExchangeService currencyExchangeRepository)
        {
            this.currencyExchangeRepository = currencyExchangeRepository;
        }

        [HttpGet]
        [Route("Convert")]
        public async Task<IActionResult> Convert([FromQuery] CurrencyExchangeRequestModel request)
        {
            var convertCurrency = new CurrencyExchangeResponseModel();

            try
            {
                convertCurrency = await currencyExchangeRepository.Convert(request.Base, request.Target, request.Amount);
            }
            catch (Exception ex)
            {
                Log.Error($"something went wrong {ex}");

                return BadRequest(ex);
            }

            return Ok(convertCurrency);
        }

        [HttpGet]
        [Route("ConversionHistory")]
        public async Task<IActionResult> ConversionHistory()
        {
            var history = await currencyExchangeRepository.ConversionHistory();

            return Ok(history);
        }
    }
}
