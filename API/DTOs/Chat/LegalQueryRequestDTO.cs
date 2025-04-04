using FluentValidation;
using Services;

namespace API.DTOs.Chat
{
    /// <summary>
    /// نموذج طلب استعلام قانوني
    /// </summary>
    public class LegalQueryRequestDTO
    {
        /// <summary>
        /// نص الاستعلام
        /// </summary>
        public string Query { get; set; } = string.Empty;
        
        /// <summary>
        /// سياق الاستعلام
        /// </summary>
        public string? Context { get; set; }
        
        /// <summary>
        /// معرفات الملفات المرجعية (اختياري)
        /// </summary>
        public List<long>? ReferenceFileIds { get; set; }
    }
    
    /// <summary>
    /// التحقق من صحة نموذج طلب استعلام قانوني
    /// </summary>
    public class LegalQueryRequestDTOValidator : AbstractValidator<LegalQueryRequestDTO>
    {
        private readonly ILocalizationService _localizationService;
        
        public LegalQueryRequestDTOValidator(ILocalizationService localizationService)
        {
            _localizationService = localizationService;
            
            RuleFor(x => x.Query)
                .NotEmpty().WithMessage(x => _localizationService.GetMessage("QueryRequired", "Validation", "ar"))
                .MaximumLength(5000).WithMessage(x => _localizationService.GetMessage("QueryTooLong", "Validation", "ar"));
                
            When(x => !string.IsNullOrEmpty(x.Context), () => {
                RuleFor(x => x.Context)
                    .MaximumLength(10000).WithMessage(x => _localizationService.GetMessage("ContextTooLong", "Validation", "ar"));
            });
            
            When(x => x.ReferenceFileIds != null && x.ReferenceFileIds.Any(), () => {
                RuleForEach(x => x.ReferenceFileIds)
                    .GreaterThan(0).WithMessage(x => _localizationService.GetMessage("InvalidFileId", "Validation", "ar"));
            });
        }
    }
} 