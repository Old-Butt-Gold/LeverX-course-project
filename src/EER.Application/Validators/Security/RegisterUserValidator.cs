using EER.Application.Dto.Security.RegisterUser;
using EER.Domain.DatabaseAbstractions;
using EER.Domain.Enums;
using FluentValidation;

namespace EER.Application.Validators.Security;

public class RegisterUserValidator : AbstractValidator<RegisterUserDto>
{
    public RegisterUserValidator(IUserRepository userRepository)
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(150)
            .MustAsync(async (email, ct) =>
                !await userRepository.IsEmailExistsAsync(email, cancellationToken: ct))
            .WithMessage("Email already exists");

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches("[A-Z]").WithMessage("The password must contain at least one capital letter")
            .Matches("[a-z]").WithMessage("The password must contain at least one lowercase letter")
            .Matches("[0-9]").WithMessage("The password must contain at least one digit");

        RuleFor(x => x.UserRole)
            .IsInEnum()
            .Must(x => x != Role.Admin);
    }
}
