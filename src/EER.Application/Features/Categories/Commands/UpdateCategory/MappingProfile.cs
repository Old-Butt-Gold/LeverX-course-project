using AutoMapper;
using EER.Domain.Entities;

namespace EER.Application.Features.Categories.Commands.UpdateCategory;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<UpdateCategoryDto, Category>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.TotalEquipment, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            // TODO fix later
            .ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.Equipment, opt => opt.Ignore());

        CreateMap<Category, CategoryUpdatedDto>();
    }
}
