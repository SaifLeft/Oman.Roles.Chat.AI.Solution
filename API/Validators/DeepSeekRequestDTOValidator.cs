using FluentValidation;
using Models;
using Services;

namespace API.Validators
{
    public class DeepSeekRequestDTOValidator : AbstractValidator<DeepSeekRequestDTO>
    {
        private readonly ILocalizationService _localizationService;

        public DeepSeekRequestDTOValidator(ILocalizationService localizationService)
        {
            _localizationService = localizationService;

            RuleFor(x => x.Model)
                .NotEmpty().WithMessage(x => _localizationService.GetMessage("ModelRequired", "Validation", "ar"))
                .Must(model => model == "deepseek-chat" || model == "deepseek-coder" || model == "deepseek-thinking").WithMessage(x => _localizationService.GetMessage("InvalidModelType", "Validation", "ar"));

            RuleFor(x => x.Messages)
                .NotEmpty().WithMessage(x => _localizationService.GetMessage("MessagesRequired", "Validation", "ar"))
                .Must(messages => messages.Count > 0).WithMessage(x => _localizationService.GetMessage("MessagesRequired", "Validation", "ar"));

            RuleForEach(x => x.Messages)
                .SetValidator(new ChatMessageDTOValidator(_localizationService));

            RuleFor(x => x.Temperature)
                .InclusiveBetween(0, 1).WithMessage(x => _localizationService.GetMessage("TemperatureRange", "Validation", "ar"));

            RuleFor(x => x.MaxTokens)
                .GreaterThan(0).WithMessage(x => _localizationService.GetMessage("MaxTokensPositive", "Validation", "ar"))
                .LessThanOrEqualTo(4000).WithMessage(x => _localizationService.GetMessage("MaxTokensRange", "Validation", "ar"));
        }
    }

    public class ChatMessageDTOValidator : AbstractValidator<ChatMessageDTO>
    {
        private readonly ILocalizationService _localizationService;

        public ChatMessageDTOValidator(ILocalizationService localizationService)
        {
            _localizationService = localizationService;

            RuleFor(x => x.Role)
                .NotEmpty().WithMessage(x => _localizationService.GetMessage("RoleRequired", "Validation", "ar"))
                .Must(role => role == "system" || role == "user" || role == "assistant").WithMessage(x => _localizationService.GetMessage("InvalidRole", "Validation", "ar"));

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage(x => _localizationService.GetMessage("ContentRequired", "Validation", "ar"))
                .MaximumLength(16000).WithMessage(x => _localizationService.GetMessage("ContentTooLong", "Validation", "ar"));
        }
    }
} 