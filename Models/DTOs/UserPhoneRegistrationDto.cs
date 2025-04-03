using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace Models.DTOs;

public class UserPhoneRegistrationDto
{
    [Required(ErrorMessage = "PhoneNumberRequired")]
    [Phone(ErrorMessage = "InvalidPhoneFormat")]
    public string PhoneNumber { get; set; }
    
    [Required(ErrorMessage = "PasswordRequired")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required(ErrorMessage = "ConfirmationCodeRequired")]
    public string ConfirmationCode { get; set; }
    public string IpAddress { get; set; }
    public string UserAgent { get; set; }
}

public class UserPhoneRegistrationDtoValidator : AbstractValidator<UserPhoneRegistrationDto>
{
    public UserPhoneRegistrationDtoValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("PhoneNumberRequired")
            .Matches("^\\+968\\d{8}$").WithMessage("InvalidOmaniNumberFormat");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("PasswordRequired")
            .MinimumLength(8).WithMessage("PasswordMinLength")
            .Matches("[A-Z]").WithMessage("PasswordUppercaseRequired")
            .Matches("[a-z]").WithMessage("PasswordLowercaseRequired")
            .Matches("[0-9]").WithMessage("PasswordNumberRequired");

        RuleFor(x => x.ConfirmationCode)
            .NotEmpty().WithMessage("ConfirmationCodeRequired")
            .Matches("^\\d{6}$").WithMessage("InvalidConfirmationCodeFormat");
    }
}