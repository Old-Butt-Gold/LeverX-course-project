using AutoMapper;
using EER.Domain.Entities;

namespace EER.Application.Features.Users.Queries.GetUserById;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDetailsDto>();
    }
}
