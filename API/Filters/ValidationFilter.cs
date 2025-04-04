using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Models.Common;
using Services;
using System;
using System.Linq;

namespace API.Filters
{
    /// <summary>
    /// فلتر التحقق من صحة البيانات
    /// </summary>
    public class ValidationFilter : IActionFilter
    {
        private readonly ILocalizationService _localizationService;
        private readonly ILogger<ValidationFilter> _logger;

        public ValidationFilter(ILocalizationService localizationService, ILogger<ValidationFilter> logger)
        {
            _localizationService = localizationService;
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                // التحقق من صحة نموذج الطلب
                if (!context.ModelState.IsValid)
                {
                    var errors = context.ModelState
                        .Where(e => e.Value.Errors.Count > 0)
                        .SelectMany(x => x.Value.Errors)
                        .Select(x => x.ErrorMessage)
                        .ToList();

                    // استخراج اللغة المفضلة
                    var language = context.HttpContext.Request.Headers["Accept-Language"].FirstOrDefault()?.Split(',')[0]?.Split(';')[0] ?? "ar";
                    
                    // إنشاء رسالة خطأ
                    var errorMessage = _localizationService.GetMessage("ValidationError", "Errors", language);
                    
                    // إنشاء استجابة فشل
                    var response = BaseResponse.FailureResponse(errorMessage, 400, errors);
                    
                    context.Result = new BadRequestObjectResult(response);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in validation filter");
                context.Result = new BadRequestObjectResult(new { error = "A validation error occurred" });
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // لا نحتاج أي إجراء هنا
        }
    }
} 