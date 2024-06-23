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
        public async Task<IActionResult> Convert([FromQuery] CurrencyExchangeRequestModel request)
        {
            var kk = new CurrencyExchangeResponseModel();

            try
            {
                kk = await currencyExchangeRepository.Convert(request.Base, request.Target, request.Amount);
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
        public async Task<IActionResult> ConversionHistory()
        {
            var history = await currencyExchangeRepository.ConversionHistory();

            return Ok(history);
        }
    }
}
