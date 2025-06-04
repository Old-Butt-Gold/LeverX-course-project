using AutoMapper;
using EER.Domain.Entities;

namespace EER.Application.Features.Categories.Commands.CreateCategory;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CreateCategoryDto, Category>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.TotalEquipment, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Equipment, opt => opt.Ignore());

        CreateMap<Category, CategoryCreatedDto>();
    }
}
