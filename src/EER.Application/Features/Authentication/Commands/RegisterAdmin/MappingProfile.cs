using AutoMapper;
using EER.Domain.Entities;
using EER.Domain.Enums;

namespace EER.Application.Features.Authentication.Commands.RegisterAdmin;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<RegisterAdminDto, User>()
            .ForMember(dest => dest.UserRole, opt => opt.MapFrom(_ => Role.Admin))
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.Equipment, opt => opt.Ignore())
            .ForMember(dest => dest.FullName, opt => opt.Ignore())
            .ForMember(dest => dest.Favorites, opt => opt.Ignore())
            .ForMember(dest => dest.Offices, opt => opt.Ignore())
            .ForMember(dest => dest.RentalCustomers, opt => opt.Ignore())
            .ForMember(dest => dest.RentalOwners, opt => opt.Ignore())
            .ForMember(dest => dest.Reviews, opt => opt.Ignore())
            .ForMember(dest => dest.RefreshTokens, opt => opt.Ignore());
    }
}
