using AutoMapper;

namespace EER.Application.Features.Equipment.Queries.GetEquipmentById;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Domain.Entities.Equipment, EquipmentDetailsDto>();
    }
}
