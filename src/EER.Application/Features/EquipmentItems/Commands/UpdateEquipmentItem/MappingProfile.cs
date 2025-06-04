using AutoMapper;
using EER.Domain.Entities;

namespace EER.Application.Features.EquipmentItems.Commands.UpdateEquipmentItem;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<UpdateEquipmentItemDto, EquipmentItem>()
            .ForMember(dest => dest.EquipmentId, opt => opt.Ignore())
            .ForMember(dest => dest.MaintenanceDate, opt => opt.MapFrom(src =>
                src.MaintenanceDate.HasValue
                    ? src.MaintenanceDate.Value.ToDateTime(TimeOnly.MinValue)
                    : (DateTime?)null))
            .ForMember(dest => dest.PurchaseDate, opt
                => opt.MapFrom(src => src.PurchaseDate.ToDateTime(TimeOnly.MinValue)))
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Equipment, opt => opt.Ignore())
            .ForMember(dest => dest.Office, opt => opt.Ignore())
            .ForMember(dest => dest.RentalItems, opt => opt.Ignore());

        CreateMap<EquipmentItem, EquipmentItemUpdatedDto>();
    }
}
