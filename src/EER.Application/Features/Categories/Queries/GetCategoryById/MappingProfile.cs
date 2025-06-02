using AutoMapper;
using EER.Domain.Entities;

namespace EER.Application.Features.Categories.Queries.GetCategoryById;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Category, CategoryDetailsDto>();
    }
}
