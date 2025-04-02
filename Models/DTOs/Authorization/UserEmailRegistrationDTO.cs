using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace Models.DTOs.Authorization
{
    public class UserEmailRegistrationDTO
    {
        public string Username { get; set; }
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public long? PhoneNumber { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
    }

    //fluent validation
    public class UserEmailRegistrationDTOValidator : AbstractValidator<UserEmailRegistrationDTO>
    {
        public UserEmailRegistrationDTOValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("UsernameRequired")
                .MinimumLength(3).WithMessage("UsernameMinLength")
                .MaximumLength(50).WithMessage("UsernameMaxLength")
                .Matches(@"^[a-zA-Z0-9._-]+$").WithMessage("UsernameInvalidFormat");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("EmailRequired")
                .EmailAddress().WithMessage("InvalidEmailFormat");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("PasswordRequired")
                .MinimumLength(8).WithMessage("PasswordMinLength")
                .Matches(@"[A-Z]+").WithMessage("PasswordUppercaseRequired")
                .Matches(@"[a-z]+").WithMessage("PasswordLowercaseRequired")
                .Matches(@"[0-9]+").WithMessage("PasswordNumberRequired")
                .Matches(@"[!@#$%^&*(),.?""{}|<>]+").WithMessage("PasswordSpecialCharRequired");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("FirstNameRequired")
                .MinimumLength(2).WithMessage("FirstNameMinLength")
                .MaximumLength(50).WithMessage("FirstNameMaxLength")
                .Matches(@"^[\p{L}\s-]+$").WithMessage("FirstNameInvalidFormat");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("LastNameRequired")
                .MinimumLength(2).WithMessage("LastNameMinLength")
                .MaximumLength(50).WithMessage("LastNameMaxLength")
                .Matches(@"^[\p{L}\s-]+$").WithMessage("LastNameInvalidFormat");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("PhoneNumberRequired")
                .Must(phone => phone.ToString().StartsWith("968") && phone.ToString().Length == 11)
                .WithMessage("InvalidOmanPhoneFormat");

            When(x => !string.IsNullOrEmpty(x.IpAddress), () =>
            {
                RuleFor(x => x.IpAddress)
                    .Matches(@"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$")
                    .WithMessage("InvalidIpAddressFormat");
            });
        }
    }

}