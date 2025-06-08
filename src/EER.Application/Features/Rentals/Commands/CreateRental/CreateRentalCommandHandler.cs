using AutoMapper;
using EER.Domain.DatabaseAbstractions;
using EER.Domain.DatabaseAbstractions.Transaction;
using EER.Domain.Entities;
using EER.Domain.Enums;
using FluentValidation;
using MediatR;

namespace EER.Application.Features.Rentals.Commands.CreateRental;

internal sealed class CreateRentalCommandHandler
    : IRequestHandler<CreateRentalCommand, RentalCreatedDto>
{
    private readonly IRentalRepository _rentalRepository;
    private readonly IEquipmentItemRepository _equipmentItemRepository;
    private readonly IMapper _mapper;
    private readonly ITransactionManager _transactionManager;

    public CreateRentalCommandHandler(IRentalRepository rentalRepository, IMapper mapper,
        IEquipmentItemRepository equipmentItemRepository, ITransactionManager transactionManager)
    {
        _rentalRepository = rentalRepository;
        _equipmentItemRepository = equipmentItemRepository;
        _mapper = mapper;
        _transactionManager = transactionManager;
    }

    public async Task<RentalCreatedDto> Handle(CreateRentalCommand command, CancellationToken cancellationToken)
    {
        await using var transaction = await _transactionManager
            .BeginTransactionAsync(ct: cancellationToken);

        try
        {
            var manipulator = command.Manipulator;
            var dto = command.CreateRentalDto;

            var equipmentItems = (await _equipmentItemRepository
                .GetByIdsWithEquipmentAsync(dto.EquipmentItemIds, transaction, cancellationToken))
                .ToList();

            ValidateEquipmentItems(equipmentItems, dto);

            var (totalPrice, rentalItems) = CalculateRentalPrice(equipmentItems, dto, manipulator);

            var rental = _mapper.Map<Rental>(dto);
            rental.TotalPrice = totalPrice;
            rental.Status = RentalStatus.Pending;
            rental.CreatedBy = manipulator;
            rental.UpdatedBy = manipulator;

            var createdRental = await _rentalRepository.AddAsync(rental, transaction, cancellationToken);

            rentalItems.ForEach(x => { x.RentalId = createdRental.Id; });

            await _rentalRepository.AddRentalItemsAsync(rentalItems, transaction, cancellationToken);

            await _equipmentItemRepository.UpdateStatusForItemsAsync(equipmentItems.Select(ei => ei.Id),
                ItemStatus.InUse, manipulator, transaction, cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return _mapper.Map<RentalCreatedDto>(createdRental);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private static void ValidateEquipmentItems(ICollection<EquipmentItem> equipmentItems, CreateRentalDto dto)
    {
        // all items exists
        if (equipmentItems.Count != dto.EquipmentItemIds.Count)
        {
            throw new ValidationException("Some equipment items not found");
        }

        var unavailableItems = equipmentItems
            .Where(ei => ei.ItemStatus != ItemStatus.Available)
            .ToList();

        // all items are available
        if (unavailableItems.Count != 0)
        {
            var itemIds = string.Join(", ", unavailableItems.Select(ei => ei.Id));
            throw new ValidationException($"Equipment items not available: {itemIds}. Status must be Available");
        }

        // all items are from only ONE owner
        var distinctOwnerIds = equipmentItems
            .Select(ei => ei.Equipment.OwnerId)
            .Distinct()
            .ToList();

        if (distinctOwnerIds.Count != 1 || distinctOwnerIds[0] != dto.OwnerId)
        {
            throw new ValidationException("All equipment items must belong to the same owner and match the rental owner");
        }
    }

    private static (decimal totalPrice, List<RentalItem> rentalItems) CalculateRentalPrice(
        IEnumerable<EquipmentItem> equipmentItems, CreateRentalDto dto, Guid manipulator)
    {
        decimal totalPrice = 0;
        var rentalItems = new List<RentalItem>();
        var rentalDays = (dto.EndDate - dto.StartDate).Days;

        foreach (var item in equipmentItems)
        {
            var itemPrice = rentalDays * item.Equipment.PricePerDay;
            totalPrice += itemPrice;

            rentalItems.Add(new RentalItem
            {
                EquipmentItemId = item.Id,
                ActualPrice = itemPrice,
                CreatedBy = manipulator,
                CreatedAt = DateTime.UtcNow
            });
        }

        return (totalPrice, rentalItems);
    }
}
