using AutoMapper;
using EER.Application.Features.Categories.Queries.GetAllCategories;
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
            // TODO Later fix
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(_ => Guid.NewGuid()))
            // TODO Later fix
            .ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.Equipment, opt => opt.Ignore());

        CreateMap<Category, CategoryCreatedDto>();
    }
}
