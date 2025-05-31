using AutoMapper;
using EER.Domain.Entities;

namespace EER.Application.Features.Offices.Queries.GetAllOffices;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Office, OfficeDto>();
    }
}
