using AutoMapper;
using EER.Domain.Entities;

namespace EER.Application.Features.Offices.Commands.UpdateOffice;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<UpdateOfficeDto, Office>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            // TODO Fix later
            .ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.EquipmentItems, opt => opt.Ignore())
            .ForMember(dest => dest.Owner, opt => opt.Ignore());

        CreateMap<Office, OfficeUpdatedDto>();
    }
}
