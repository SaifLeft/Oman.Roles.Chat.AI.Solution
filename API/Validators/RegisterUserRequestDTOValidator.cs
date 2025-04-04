using FluentValidation;
using Models.DTOs.Authorization;
using Services;

namespace API.Validators
{
    public class RegisterUserRequestDTOValidator : AbstractValidator<RegisterUserRequestDTO>
    {
        private readonly ILocalizationService _localizationService;

        public RegisterUserRequestDTOValidator(ILocalizationService localizationService)
        {
            _localizationService = localizationService;

            RuleFor(x => x.Username)
                .NotEmpty().WithMessage(x => _localizationService.GetMessage("UsernameRequired", "Validation", "ar"))
                .MinimumLength(3).WithMessage(x => _localizationService.GetMessage("UsernameMinLength", "Validation", "ar"))
                .MaximumLength(50).WithMessage(x => _localizationService.GetMessage("UsernameMaxLength", "Validation", "ar"))
                .Matches(@"^[a-zA-Z0-9._-]+$").WithMessage(x => _localizationService.GetMessage("UsernameInvalidFormat", "Validation", "ar"));

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(x => _localizationService.GetMessage("EmailRequired", "Validation", "ar"))
                .EmailAddress().WithMessage(x => _localizationService.GetMessage("InvalidEmailFormat", "Validation", "ar"));

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(x => _localizationService.GetMessage("PasswordRequired", "Validation", "ar"))
                .MinimumLength(8).WithMessage(x => _localizationService.GetMessage("PasswordMinLength", "Validation", "ar"))
                .Matches(@"[A-Z]+").WithMessage(x => _localizationService.GetMessage("PasswordUppercaseRequired", "Validation", "ar"))
                .Matches(@"[a-z]+").WithMessage(x => _localizationService.GetMessage("PasswordLowercaseRequired", "Validation", "ar"))
                .Matches(@"[0-9]+").WithMessage(x => _localizationService.GetMessage("PasswordNumberRequired", "Validation", "ar"))
                .Matches(@"[!@#$%^&*(),.?""{}|<>]+").WithMessage(x => _localizationService.GetMessage("PasswordSpecialCharRequired", "Validation", "ar"));

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage(x => _localizationService.GetMessage("FirstNameRequired", "Validation", "ar"))
                .MinimumLength(2).WithMessage(x => _localizationService.GetMessage("FirstNameMinLength", "Validation", "ar"))
                .MaximumLength(50).WithMessage(x => _localizationService.GetMessage("FirstNameMaxLength", "Validation", "ar"));

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage(x => _localizationService.GetMessage("LastNameRequired", "Validation", "ar"))
                .MinimumLength(2).WithMessage(x => _localizationService.GetMessage("LastNameMinLength", "Validation", "ar"))
                .MaximumLength(50).WithMessage(x => _localizationService.GetMessage("LastNameMaxLength", "Validation", "ar"));

            RuleFor(x => x.PhoneNumber)
                .Must(phoneNumber => phoneNumber == null || phoneNumber > 0)
                .WithMessage(x => _localizationService.GetMessage("InvalidPhoneNumber", "Validation", "ar"));
        }
    }
} 