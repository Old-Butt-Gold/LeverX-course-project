using AutoMapper;
using EER.Domain.Entities;

namespace EER.Application.Features.Reviews.Commands.CreateReview;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CreateReviewDto, Review>()
            .ForMember(dest => dest.CustomerId, opt => opt.Ignore())
            .ForMember(dest => dest.EquipmentId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Equipment, opt => opt.Ignore())
            .ForMember(dest => dest.Customer, opt => opt.Ignore());

        CreateMap<Review, ReviewCreatedDto>();
    }
}
