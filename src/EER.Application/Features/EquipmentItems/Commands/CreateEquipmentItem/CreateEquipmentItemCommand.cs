using EER.Domain.Entities;
using EER.Domain.Enums;
using MediatR;

namespace EER.Application.Features.EquipmentItems.Commands.CreateEquipmentItem;

public record CreateEquipmentItemCommand(
    int EquipmentId,
    int? OfficeId,
    string SerialNumber,
    ItemStatus ItemStatus,
    DateTime? MaintenanceDate,
    DateTime PurchaseDate) : IRequest<EquipmentItem>;
