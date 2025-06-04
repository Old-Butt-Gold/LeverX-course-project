using MediatR;

namespace EER.Application.Features.Categories.Commands.CreateCategory;

public record CreateCategoryCommand(CreateCategoryDto CreateCategoryDto, Guid Manipulator) : IRequest<CategoryCreatedDto>;
