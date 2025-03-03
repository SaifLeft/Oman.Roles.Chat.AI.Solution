using Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services;

namespace API.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ChatRulesController : ControllerBase
    {
        private readonly IChatRulesService _chatRulesService;
        private readonly ILogger<ChatRulesController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ILocalizationService _localizationService;

        public ChatRulesController(
            IChatRulesService chatRulesService,
            ILogger<ChatRulesController> logger,
            IConfiguration configuration,
            ILocalizationService localizationService)
        {
            _chatRulesService = chatRulesService;
            _logger = logger;
            _configuration = configuration;
            _localizationService = localizationService;
        }

        /// <summary>
        /// الحصول على القواعد الافتراضية
        /// </summary>
        [HttpGet("default")]
        public IActionResult GetDefaultRules()
        {
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);

            try
            {
                var rules = _chatRulesService.GetDefaultRules(language);
                return Ok(BaseResponse.SuccessResponse(rules));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء الحصول على القواعد الافتراضية");
                var errorMessage = _localizationService.GetMessage("RulesRetrievalError", "Errors", language);
                return StatusCode(500, BaseResponse.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// تحديث القواعد الافتراضية
        /// </summary>
        [HttpPut("default")]
        public IActionResult UpdateDefaultRules([FromBody] UpdateRulesRequest request)
        {
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);

            if (string.IsNullOrEmpty(request.Rules))
            {
                var errorMessage = _localizationService.GetMessage("RulesContentRequired", "Errors", language);
                return BadRequest(BaseResponse.FailureResponse(errorMessage, 400));
            }

            try
            {
                var success = _chatRulesService.UpdateDefaultRules(request.Rules, language);
                if (success)
                {
                    var successMessage = _localizationService.GetMessage("RulesUpdatedSuccess", "Messages", language);
                    return Ok(BaseResponse<bool>.SuccessResponse(true, successMessage));
                }
                else
                {
                    var errorMessage = _localizationService.GetMessage("RulesUpdateFailed", "Errors", language);
                    return StatusCode(500, BaseResponse.FailureResponse(errorMessage, 500));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء تحديث القواعد الافتراضية");
                var errorMessage = _localizationService.GetMessage("RulesUpdateError", "Errors", language);
                return StatusCode(500, BaseResponse.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// الحصول على قائمة القواعد المتاحة
        /// </summary>
        [HttpGet]
        public IActionResult GetAvailableRulesets()
        {
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);

            try
            {
                var rulesets = _chatRulesService.GetAvailableRulesets();
                return Ok(BaseResponse<Dictionary<string, string>>.SuccessResponse(rulesets));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء الحصول على قائمة القواعد المتاحة");
                var errorMessage = _localizationService.GetMessage("RulesetsRetrievalError", "Errors", language);
                return StatusCode(500, BaseResponse.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// إضافة مجموعة قواعد جديدة
        /// </summary>
        [HttpPost]
        public IActionResult AddRuleset([FromBody] AddRulesetRequest request)
        {
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);

            if (string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.Rules))
            {
                var errorMessage = _localizationService.GetMessage("RulesetNameAndContentRequired", "Errors", language);
                return BadRequest(BaseResponse.FailureResponse(errorMessage, 400));
            }

            try
            {
                var success = _chatRulesService.AddRuleset(request.Name, request.Rules);
                if (success)
                {
                    var successMessage = _localizationService.GetMessage("RulesetAddedSuccess", "Messages", language);
                    return Ok(BaseResponse<bool>.SuccessResponse(true, successMessage));
                }
                else
                {
                    var errorMessage = _localizationService.GetMessage("RulesetAddFailed", "Errors", language);
                    return StatusCode(500, BaseResponse.FailureResponse(errorMessage, 500));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء إضافة مجموعة قواعد جديدة");
                var errorMessage = _localizationService.GetMessage("RulesetAddError", "Errors", language);
                return StatusCode(500, BaseResponse.FailureResponse(errorMessage, 500));
            }
        }

        /// <summary>
        /// حذف مجموعة قواعد
        /// </summary>
        [HttpDelete("{name}")]
        public IActionResult DeleteRuleset(string name)
        {
            string language = LanguageHelper.GetPreferredLanguage(Request, _configuration);

            if (string.IsNullOrEmpty(name))
            {
                var errorMessage = _localizationService.GetMessage("RulesetNameRequired", "Errors", language);
                return BadRequest(BaseResponse.FailureResponse(errorMessage, 400));
            }

            try
            {
                var success = _chatRulesService.DeleteRuleset(name);
                if (success)
                {
                    var successMessage = _localizationService.GetMessage("RulesetDeletedSuccess", "Messages", language);
                    return Ok(BaseResponse<bool>.SuccessResponse(true, successMessage));
                }
                else
                {
                    var errorMessage = _localizationService.GetMessage("RulesetDeleteFailed", "Errors", language);
                    return StatusCode(404, BaseResponse.FailureResponse(errorMessage, 404));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء حذف مجموعة القواعد");
                var errorMessage = _localizationService.GetMessage("RulesetDeleteError", "Errors", language);
                return StatusCode(500, BaseResponse.FailureResponse(errorMessage, 500));
            }
        }
    }

    public class UpdateRulesRequest
    {
        public string Rules { get; set; } = string.Empty;
    }

    public class AddRulesetRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Rules { get; set; } = string.Empty;
    }
}