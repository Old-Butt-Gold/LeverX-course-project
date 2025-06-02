using AutoMapper;
using EER.Domain.Entities;

namespace EER.Application.Features.Users.Queries.GetAllUsers;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>();
    }
}
