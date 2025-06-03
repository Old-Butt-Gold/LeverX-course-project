using EER.Domain.DatabaseAbstractions;
using FluentValidation;

namespace EER.Application.Features.Users.Commands.UpdateUser;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator(IUserRepository userRepository)
    {
        RuleFor(x => x.UpdateUserDto.Id)
            .NotEmpty();

        RuleFor(x => x.UpdateUserDto.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(150)
            .MustAsync(async (email, ct) =>
                !await userRepository.IsEmailExists(email, cancellationToken: ct))
            .WithMessage("Email already exists");

        RuleFor(x => x.UpdateUserDto.FullName)
            .MaximumLength(150);
    }
}
