using FluentValidation;
using Models.Common;

namespace API.Validation
{
    /// <summary>
    /// التحقق من صحة طلبات إحصائيات لوحة التحكم
    /// </summary>
    public class AdminAnalyticsQueryValidator : AbstractValidator<AnalyticsPeriodQuery>
    {
        public AdminAnalyticsQueryValidator()
        {
            RuleFor(x => x.FromDate)
                .NotEmpty().WithMessage("يجب تحديد تاريخ البداية")
                .LessThanOrEqualTo(x => x.ToDate).WithMessage("يجب أن يكون تاريخ البداية قبل تاريخ النهاية");

            RuleFor(x => x.ToDate)
                .NotEmpty().WithMessage("يجب تحديد تاريخ النهاية")
                .GreaterThanOrEqualTo(x => x.FromDate).WithMessage("يجب أن يكون تاريخ النهاية بعد تاريخ البداية");
                
            RuleFor(x => x.Language)
                .NotEmpty().WithMessage("يجب تحديد اللغة")
                .Must(lang => lang == "ar" || lang == "en").WithMessage("اللغة يجب أن تكون 'ar' أو 'en'");
        }
    }
} 