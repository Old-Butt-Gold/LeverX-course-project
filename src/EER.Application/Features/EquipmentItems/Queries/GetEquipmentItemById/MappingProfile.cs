using AutoMapper;
using EER.Domain.Entities;

namespace EER.Application.Features.EquipmentItems.Queries.GetEquipmentItemById;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<EquipmentItem, EquipmentItemDetailsDto>();
    }
}
