using AutoMapper;

namespace EER.Application.Features.Equipment.Commands.UpdateEquipment;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<UpdateEquipmentDto, Domain.Entities.Equipment>()
            .ForMember(dest => dest.IsModerated, opt => opt.Ignore())
            .ForMember(dest => dest.OwnerId, opt => opt.Ignore())
            .ForMember(dest => dest.AverageRating, opt => opt.Ignore())
            .ForMember(dest => dest.TotalReviews, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Category, opt => opt.Ignore())
            .ForMember(dest => dest.EquipmentImages, opt => opt.Ignore())
            .ForMember(dest => dest.EquipmentItems, opt => opt.Ignore())
            .ForMember(dest => dest.Favorites, opt => opt.Ignore())
            .ForMember(dest => dest.Owner, opt => opt.Ignore())
            .ForMember(dest => dest.Reviews, opt => opt.Ignore());

        CreateMap<Domain.Entities.Equipment, EquipmentUpdatedDto>();
    }
}
