using EER.Domain.DatabaseAbstractions;
using MediatR;

namespace EER.Application.Features.Rentals.Queries.GetDetailedRental;

internal sealed class GetDetailedRentalQueryHandler
    : IRequestHandler<GetDetailedRentalQuery, DetailedRentalDto?>
{
    private readonly IRentalRepository _rentalRepository;
    private readonly IEquipmentItemRepository _equipmentItemRepository;

    public GetDetailedRentalQueryHandler(IRentalRepository rentalRepository, IEquipmentItemRepository equipmentItemRepository)
    {
        _rentalRepository = rentalRepository;
        _equipmentItemRepository = equipmentItemRepository;
    }

    public async Task<DetailedRentalDto?> Handle(GetDetailedRentalQuery request, CancellationToken cancellationToken)
    {
        var rentalId = request.Id;

        // TODO Check after insert

        var rental = await _rentalRepository.GetByIdWithItemsAsync(
            rentalId, cancellationToken: cancellationToken);

        if (rental is null ||
            (rental.CustomerId != request.UserId && rental.OwnerId != request.UserId))
        {
            return null;
        }

        var itemIds = rental.RentalItems
            .Select(ri => ri.EquipmentItemId);

        var equipmentItems = await _equipmentItemRepository.GetByIdsWithEquipmentAsync(
            itemIds, cancellationToken: cancellationToken);

        var equipmentItemDict = equipmentItems.ToDictionary(ei => ei.Id);

        return new DetailedRentalDto
        {
            Id = rental.Id,
            StartDate = rental.StartDate,
            EndDate = rental.EndDate,
            TotalPrice = rental.TotalPrice,
            Status = rental.Status,
            Items = rental.RentalItems.Select(ri =>
            {
                var item = equipmentItemDict[ri.EquipmentItemId];
                return new DetailedEquipmentItemDto
                {
                    EquipmentItemId = ri.EquipmentItemId,
                    SerialNumber = item.SerialNumber,
                    ActualPrice = ri.ActualPrice,
                    EquipmentName = item.Equipment.Name,
                    PurchaseDate = item.PurchaseDate
                };
            }).ToList()
        };
    }
}
