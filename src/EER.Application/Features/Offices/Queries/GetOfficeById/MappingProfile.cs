using AutoMapper;
using EER.Domain.Entities;

namespace EER.Application.Features.Offices.Queries.GetOfficeById;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Office, OfficeDetailsDto>();
    }
}
