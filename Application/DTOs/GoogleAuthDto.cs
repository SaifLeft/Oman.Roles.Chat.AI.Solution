using FluentValidation;

namespace Application.DTOs.Authentication;

public class GoogleAuthDto
{
    public string Token { get; set; }
}

public class GoogleAuthDtoValidator : AbstractValidator<GoogleAuthDto>
{
    public GoogleAuthDtoValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("GoogleTokenRequired")
            .Must(t => t.Split('.').Length == 3).WithMessage("InvalidGoogleTokenFormat");
    }
}