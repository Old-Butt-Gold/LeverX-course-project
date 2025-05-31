using AutoMapper;
using EER.Domain.Entities;

namespace EER.Application.Features.EquipmentItems.Queries.GetAllEquipmentItems;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<EquipmentItem, EquipmentItemDto>();
    }
}
