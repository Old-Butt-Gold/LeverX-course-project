using FluentValidation;

namespace EER.Application.Features.Users.Commands.UpdateUser;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.UpdateUserDto.Id)
            .NotEmpty();

        RuleFor(x => x.UpdateUserDto.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(150);

        RuleFor(x => x.UpdateUserDto.FullName)
            .MaximumLength(150);
    }
}
