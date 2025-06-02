using AutoMapper;
using EER.Domain.Entities;

namespace EER.Application.Features.Rentals.Queries.GetAllRentals;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Rental, RentalDto>();
    }
}
