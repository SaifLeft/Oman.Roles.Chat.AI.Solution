using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace Models.DTOs;

public class UserEmailRegistrationDto
{
    [Required(ErrorMessage = "UsernameRequired")]
    public string Username { get; set; }
    
    [Required(ErrorMessage = "EmailRequired")]
    [EmailAddress(ErrorMessage = "InvalidEmailFormat")]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "PasswordRequired")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    
    [Required(ErrorMessage = "FullNameRequired")]
    public string FullName { get; set; }
    
    public long? PhoneNumber { get; set; }
    public string IpAddress { get; set; }
    public string UserAgent { get; set; }
}

public class UserEmailRegistrationDtoValidator : AbstractValidator<UserEmailRegistrationDto>
{
    public UserEmailRegistrationDtoValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("UsernameRequired")
            .MinimumLength(3).WithMessage("UsernameMinLength")
            .MaximumLength(50).WithMessage("UsernameMaxLength");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("EmailRequired")
            .EmailAddress().WithMessage("InvalidEmailFormat");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("PasswordRequired")
            .MinimumLength(8).WithMessage("PasswordMinLength")
            .Matches("[A-Z]").WithMessage("PasswordUppercaseRequired")
            .Matches("[a-z]").WithMessage("PasswordLowercaseRequired")
            .Matches("[0-9]").WithMessage("PasswordNumberRequired");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("FullNameRequired")
            .MinimumLength(3).WithMessage("FullNameMinLength")
            .MaximumLength(100).WithMessage("FullNameMaxLength");
    }
}