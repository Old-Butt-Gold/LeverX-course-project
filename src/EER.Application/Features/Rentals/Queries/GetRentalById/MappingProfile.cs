using AutoMapper;
using EER.Domain.Entities;

namespace EER.Application.Features.Rentals.Queries.GetRentalById;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Rental, RentalDetailsDto>();
    }
}
