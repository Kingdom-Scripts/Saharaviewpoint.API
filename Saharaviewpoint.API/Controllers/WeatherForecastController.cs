using Microsoft.AspNetCore.Mvc;
using Saharaviewpoint.API;
using Saharaviewpoint.Core.Extensions;
using Saharaviewpoint.Core.Models.App;
using Saharaviewpoint.Core.Models.Utilities;

namespace Saharaviewpoint.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : BaseController
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly ShareviewpointContext _context;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, ShareviewpointContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResult))]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(SuccessResult<List<WeatherForecast>>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResult))]
        public async Task<IActionResult> GetAsync(int id)
        {
            //throw new AggregateException(new NullReferenceException());

            var result = await Enumerable.Range(1, 50).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            }).ToPaginatedListAsync(1, 10);

            var response = new Result();
            switch (id)
            {
                case 1:
                    response = new SuccessResult();
                    break;
                case 2:
                    response = new SuccessResult<List<WeatherForecast>>("Here is your forecast");
                    break;
                case 3:
                    response = new SuccessResult<List<WeatherForecast>>("Here is your forecast", "An additional message");
                    break;
                case 4:
                    response = new SuccessResult<List<WeatherForecast>>("Here is your forecast", result);
                    break;
                case 5:
                    response = new SuccessResult<List<WeatherForecast>>(StatusCodes.Status204NoContent, "Here is your forecast");
                    break;
                case 6:
                    response = new SuccessResult<List<WeatherForecast>>(StatusCodes.Status204NoContent, "Here is your forecast", "Additional no content message");
                    break;
                case 7:
                    response = new SuccessResult<List<WeatherForecast>>(StatusCodes.Status208AlreadyReported, "Here is your forecast", result);
                    break;
                case 8:
                    response = new SuccessResult<List<WeatherForecast>>(StatusCodes.Status208AlreadyReported, "Here is your forecast", "Additional content response", result);
                    break;
                case 9:
                    response = new SuccessResult<List<WeatherForecast>>(result);
                    break;
                default:
                    response = new SuccessResult<object>("Successful");
                    break;
            }

            return ProcessResponse(response);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = _context.Roles.ToList();
            throw new NotImplementedException();
            var res = new SuccessResult<List<Role>>(result);
            return ProcessResponse(res);
        }
    }
}