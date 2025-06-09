using AutoMapper;

namespace EER.Application.Features.Equipment.Queries.GetUnmoderatedEquipment;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Domain.Entities.Equipment, EquipmentForModerationDto>();
    }
}
