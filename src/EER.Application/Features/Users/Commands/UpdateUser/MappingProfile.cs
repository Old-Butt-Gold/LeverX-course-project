﻿using AutoMapper;
using EER.Domain.Entities;

namespace EER.Application.Features.Users.Commands.UpdateUser;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<UpdateUserDto, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.UserRole, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Equipment, opt => opt.Ignore())
            .ForMember(dest => dest.Favorites, opt => opt.Ignore())
            .ForMember(dest => dest.Offices, opt => opt.Ignore())
            .ForMember(dest => dest.RentalCustomers, opt => opt.Ignore())
            .ForMember(dest => dest.RentalOwners, opt => opt.Ignore())
            .ForMember(dest => dest.Reviews, opt => opt.Ignore())
            .ForMember(dest => dest.RefreshTokens, opt => opt.Ignore());

        CreateMap<User, UserUpdatedDto>();
    }
}
