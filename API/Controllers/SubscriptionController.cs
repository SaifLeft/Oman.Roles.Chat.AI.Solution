using API.Services;
using Helpers;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly ILogger<SubscriptionController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ILocalizationService _localizationService;

        public SubscriptionController(
            ISubscriptionService subscriptionService,
            ILogger<SubscriptionController> logger,
            IConfiguration configuration,
            ILocalizationService localizationService)
        {
            _subscriptionService = subscriptionService;
            _logger = logger;
            _configuration = configuration;
            _localizationService = localizationService;
        }

        /// <summary>
        /// الحصول على جميع خطط الاشتراك
        /// </summary>
        [HttpGet("plans")]
        public async Task<IActionResult> GetAllPlans()
        {
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);

            try
            {
                var result = await _subscriptionService.GetAllPlansAsync(language);
                return StatusCode(result.StatusCode