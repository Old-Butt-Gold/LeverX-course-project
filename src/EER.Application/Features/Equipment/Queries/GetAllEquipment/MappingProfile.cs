using AutoMapper;

namespace EER.Application.Features.Equipment.Queries.GetAllEquipment;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Domain.Entities.Equipment, EquipmentDto>();
    }
}
