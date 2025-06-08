using AutoMapper;
using EER.Domain.Entities;

namespace EER.Application.Features.Reviews.Queries.GetReviewsByEquipmentId;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Review, ReviewWithUserDto>()
            .ForMember(dest => dest.CustomerFullName,
                opt => opt.MapFrom(src => src.Customer.FullName));
    }
}
