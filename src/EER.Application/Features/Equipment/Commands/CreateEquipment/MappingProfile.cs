using AutoMapper;

namespace EER.Application.Features.Equipment.Commands.CreateEquipment;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CreateEquipmentDto, Domain.Entities.Equipment>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(_ => 0m))
            .ForMember(dest => dest.TotalReviews, opt => opt.MapFrom(_ => 0))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            // TODO Fix Later
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(_ => Guid.NewGuid()))
            // TODO Fix Later
            .ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.IsModerated, opt => opt.MapFrom(_ => false))
            .ForMember(dest => dest.Category, opt => opt.Ignore())
            .ForMember(dest => dest.EquipmentImages, opt => opt.Ignore())
            .ForMember(dest => dest.EquipmentItems, opt => opt.Ignore())
            .ForMember(dest => dest.Favorites, opt => opt.Ignore())
            .ForMember(dest => dest.Owner, opt => opt.Ignore())
            .ForMember(dest => dest.Reviews, opt => opt.Ignore());

        CreateMap<Domain.Entities.Equipment, EquipmentCreatedDto>();
    }
}
