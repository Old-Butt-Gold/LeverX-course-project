using EER.Application.Dto.Security.Login;
using FluentValidation;

namespace EER.Application.Validators.Security;

public class LoginUserValidator : AbstractValidator<LoginUserDto>
{
    public LoginUserValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(150);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches("[A-Z]").WithMessage("The password must contain at least one capital letter")
            .Matches("[a-z]").WithMessage("The password must contain at least one lowercase letter")
            .Matches("[0-9]").WithMessage("The password must contain at least one digit");
    }
}
