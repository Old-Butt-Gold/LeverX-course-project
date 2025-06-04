using EER.Domain.DatabaseAbstractions;
using FluentValidation;

namespace EER.Application.Features.Authentication.Commands.RegisterAdmin;

public class RegisterAdminCommandValidator : AbstractValidator<RegisterAdminCommand>
{
    public RegisterAdminCommandValidator(IUserRepository userRepository)
    {
        RuleFor(x => x.AdminDto.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(150)
            .MustAsync(async (email, ct) =>
                !await userRepository.IsEmailExistsAsync(email, cancellationToken: ct))
            .WithMessage("Email already exists");

        RuleFor(x => x.AdminDto.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches("[A-Z]").WithMessage("The password must contain at least one capital letter")
            .Matches("[a-z]").WithMessage("The password must contain at least one lowercase letter")
            .Matches("[0-9]").WithMessage("The password must contain at least one digit");
    }
}
