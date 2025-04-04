using FluentValidation;
using System.Text.Json.Serialization;

namespace Models.DTOs.Subscription
{
    /// <summary>
    /// نموذج خطة الاشتراك
    /// Subscription plan model
    /// </summary>
    public class SubscriptionPlanDTO
    {
        /// <summary>
        /// معرف خطة الاشتراك
        /// Subscription plan ID
        /// </summary>
        [JsonPropertyName("id")]
        public long Id { get; set; }

        /// <summary>
        /// اسم خطة الاشتراك
        /// Subscription plan name
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// وصف خطة الاشتراك
        /// Subscription plan description
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// السعر الشهري (بالريال العماني)
        /// Monthly price
        /// </summary>
        [JsonPropertyName("monthlyPrice")]
        public decimal MonthlyPrice { get; set; }

        /// <summary>
        /// السعر السنوي (بالريال العماني)
        /// Yearly price
        /// </summary>
        [JsonPropertyName("yearlyPrice")]
        public decimal YearlyPrice { get; set; }

        /// <summary>
        /// عدد الاستعلامات المسموح بها شهرياً
        /// Number of allowed queries per month
        /// </summary>
        [JsonPropertyName("monthlyQueryLimit")]
        public int MonthlyQueryLimit { get; set; }

        /// <summary>
        /// حجم الملفات المسموح برفعها (بالميجابايت)
        /// Allowed file upload size in MB
        /// </summary>
        [JsonPropertyName("fileUploadSizeLimitMB")]
        public int FileUploadSizeLimitMB { get; set; }

        /// <summary>
        /// هل تتضمن الخطة دعم أولوية؟
        /// Does the plan include priority support
        /// </summary>
        [JsonPropertyName("hasPrioritySupport")]
        public bool HasPrioritySupport { get; set; }

        /// <summary>
        /// هل الخطة نشطة؟
        /// Is the plan active
        /// </summary>
        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// عدد غرف الدردشة المسموح بها
        /// Number of allowed chat rooms
        /// </summary>
        [JsonPropertyName("allowedChatRooms")]
        public int AllowedChatRooms { get; set; }

        /// <summary>
        /// عدد الملفات المسموح بها
        /// Number of allowed files
        /// </summary>
        [JsonPropertyName("allowedFiles")]
        public int AllowedFiles { get; set; }

        /// <summary>
        /// الحجم المسموح به للملفات بالميجابايت
        /// Allowed file size in MB
        /// </summary>
        [JsonPropertyName("allowedFileSizeMb")]
        public int AllowedFileSizeMb { get; set; }

        /// <summary>
        /// هل الخطة تجريبية
        /// Is the plan a trial plan
        /// </summary>
        [JsonPropertyName("isTrial")]
        public bool IsTrial { get; set; }

        /// <summary>
        /// مميزات الخطة
        /// Plan features
        /// </summary>
        [JsonPropertyName("features")]
        public List<PlanFeatureDTO>? Features { get; set; }

        /// <summary>
        /// علامات الخطة
        /// Plan tags
        /// </summary>
        [JsonPropertyName("tags")]
        public List<string> Tags { get; set; } = new List<string>();

        /// <summary>
        /// عدد أيام الفترة التجريبية
        /// Number of trial days
        /// </summary>
        [JsonPropertyName("trialDays")]
        public int? TrialDays { get; set; }
    }

    /// <summary>
    /// يمثل ميزة في خطة اشتراك
    /// </summary>
    public class PlanFeatureDTO
    {
        /// <summary>
        /// معرف الميزة
        /// Feature ID
        /// </summary>
        [JsonPropertyName("id")]
        public long Id { get; set; }

        /// <summary>
        /// اسم الميزة
        /// Feature name
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// وصف الميزة
        /// Feature description
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;
    }

    /// <summary>
    /// مدقق صحة خطة الاشتراك
    /// </summary>
    public class SubscriptionPlanDTOValidator : AbstractValidator<SubscriptionPlanDTO>
    {
        public SubscriptionPlanDTOValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("يجب إدخال اسم خطة الاشتراك")
                .MaximumLength(50).WithMessage("يجب ألا يتجاوز اسم الخطة 50 حرفاً");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("يجب إدخال وصف خطة الاشتراك")
                .MaximumLength(500).WithMessage("يجب ألا يتجاوز وصف الخطة 500 حرف");

            RuleFor(x => x.MonthlyPrice)
                .GreaterThanOrEqualTo(0).WithMessage("يجب أن يكون السعر الشهري قيمة موجبة");

            RuleFor(x => x.YearlyPrice)
                .GreaterThanOrEqualTo(0).WithMessage("يجب أن يكون السعر السنوي قيمة موجبة");

            RuleFor(x => x.MonthlyQueryLimit)
                .GreaterThanOrEqualTo(0).WithMessage("يجب أن يكون حد الاستعلامات الشهرية قيمة موجبة");

            RuleFor(x => x.FileUploadSizeLimitMB)
                .GreaterThanOrEqualTo(0).WithMessage("يجب أن يكون حد حجم الملفات قيمة موجبة");
        }
    }
}