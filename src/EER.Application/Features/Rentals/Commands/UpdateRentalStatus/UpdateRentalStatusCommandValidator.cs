using EER.Domain.DatabaseAbstractions;
using EER.Domain.Enums;
using FluentValidation;

namespace EER.Application.Features.Rentals.Commands.UpdateRentalStatus;

public class UpdateRentalStatusCommandValidator : AbstractValidator<UpdateRentalStatusCommand>
{
    public UpdateRentalStatusCommandValidator(IRentalRepository repository)
    {
        RuleFor(x => x.UpdateRentalDto.Id)
            .GreaterThan(0).WithMessage("Invalid rental ID");

        RuleFor(x => x.UpdateRentalDto.Status)
            .IsInEnum()
            .MustAsync(async (command, _, ct) =>
            {
                var rent = await repository.GetByIdAsync(command.UpdateRentalDto.Id, cancellationToken: ct);

                if (rent is null)
                    return true;

                return rent.Status != RentalStatus.Canceled && rent.Status != RentalStatus.Completed;
            })
            .WithMessage("You can't update status of ended rental");
    }
}
