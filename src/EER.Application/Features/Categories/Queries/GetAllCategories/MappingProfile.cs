using AutoMapper;
using EER.Domain.Entities;

namespace EER.Application.Features.Categories.Queries.GetAllCategories;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Category, CategoryDto>();
    }
}
