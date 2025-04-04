using FluentValidation;
using Services;

namespace API.DTOs.Chat
{
    /// <summary>
    /// نموذج طلب استعلام المحادثة
    /// </summary>
    public class ChatQueryRequestDTO
    {
        /// <summary>
        /// نص الاستعلام
        /// </summary>
        public string Query { get; set; } = string.Empty;
        
        /// <summary>
        /// معرف المحادثة (اختياري)
        /// </summary>
        public long? ConversationId { get; set; }
        
        /// <summary>
        /// معرفات الملفات المرفقة (اختياري)
        /// </summary>
        public List<long>? AttachmentIds { get; set; }
    }
    
    /// <summary>
    /// التحقق من صحة نموذج طلب استعلام المحادثة
    /// </summary>
    public class ChatQueryRequestDTOValidator : AbstractValidator<ChatQueryRequestDTO>
    {
        private readonly ILocalizationService _localizationService;
        
        public ChatQueryRequestDTOValidator(ILocalizationService localizationService)
        {
            _localizationService = localizationService;
            
            RuleFor(x => x.Query)
                .NotEmpty().WithMessage(x => _localizationService.GetMessage("QueryRequired", "Validation", "ar"))
                .MaximumLength(5000).WithMessage(x => _localizationService.GetMessage("QueryTooLong", "Validation", "ar"));
                
            When(x => x.ConversationId.HasValue, () => {
                RuleFor(x => x.ConversationId)
                    .GreaterThan(0).WithMessage(x => _localizationService.GetMessage("InvalidConversationId", "Validation", "ar"));
            });
            
            When(x => x.AttachmentIds != null && x.AttachmentIds.Any(), () => {
                RuleForEach(x => x.AttachmentIds)
                    .GreaterThan(0).WithMessage(x => _localizationService.GetMessage("InvalidAttachmentId", "Validation", "ar"));
            });
        }
    }
} 