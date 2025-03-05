using Microsoft.AspNetCore.Mvc;
using Services;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class DeepSeekController : ControllerBase
    {
        private readonly IDeepSeekService _deepSeekService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger<DeepSeekController> _logger;

        public DeepSeekController(
            IDeepSeekService deepSeekService,
            ILocalizationService localizationService,
            ILogger<DeepSeekController> logger)
        {
            _deepSeekService = deepSeekService;
            _localizationService = localizationService;
            _logger = logger;
        }
        [HttpPost]
        public async Task<IActionResult> ProcessQuery([FromBody] QueryRequest request)
        {
            // Get preferred language from request header or default to English
            var language = Request.Headers.AcceptLanguage.FirstOrDefault()?.Split(',')[0]?.Split(';')[0] ?? "en";

            try
            {
                _logger.LogInformation("Received process request");

                if (string.IsNullOrEmpty(request.Query))
                {
                    var errorMessage = _localizationService.GetMessage("EmptyQuery", "Errors", language);
                    return BadRequest(new { error = errorMessage });
                }

                // Log that we're processing the request
                // var processingMessage = _localizationService.GetMessage("ProcessingRequest", "Messages", language);
                //_logger.LogInformation(processingMessage);

                var result = await _deepSeekService.ProcessPdfDataAsync(request.Query);

                // Log that we've completed processing
                // var completedMessage = _localizationService.GetMessage("RequestCompleted", "Messages", language);
                // _logger.LogInformation(completedMessage);

                return Ok(new { response = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing query");
                var errorMessage = _localizationService.GetMessage("ApiError", "Errors", language);
                return StatusCode(500, new { error = errorMessage });
            }
        }
    }

    public class QueryRequest
    {
        public string Query { get; set; }
    }
}