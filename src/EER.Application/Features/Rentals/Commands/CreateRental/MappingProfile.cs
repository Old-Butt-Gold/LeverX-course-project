using AutoMapper;
using EER.Domain.Entities;
using EER.Domain.Enums;

namespace EER.Application.Features.Rentals.Commands.CreateRental;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CreateRentalDto, Rental>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => RentalStatus.Pending))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            // TODO Fix Later
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(_ => Guid.NewGuid()))
            // TODO Fix Later
            .ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.Customer, opt => opt.Ignore())
            .ForMember(dest => dest.Owner, opt => opt.Ignore())
            .ForMember(dest => dest.RentalItems, opt => opt.Ignore());

        CreateMap<Rental, RentalCreatedDto>();
    }
}
