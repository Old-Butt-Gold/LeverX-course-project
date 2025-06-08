using AutoMapper;
using EER.Domain.Entities;

namespace EER.Application.Features.Rentals.Queries.GetMyRentals;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Rental, MyRentalDto>();
    }
}
