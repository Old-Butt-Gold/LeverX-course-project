using MediatR;

namespace EER.Application.Features.Categories.Commands.UpdateCategory;

public record UpdateCategoryCommand(UpdateCategoryDto UpdateCategoryDto, Guid Manipulator) : IRequest<CategoryUpdatedDto>;
