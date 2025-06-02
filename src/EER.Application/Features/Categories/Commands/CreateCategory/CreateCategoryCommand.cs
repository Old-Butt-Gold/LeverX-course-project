using EER.Application.Features.Categories.Queries.GetAllCategories;
using MediatR;

namespace EER.Application.Features.Categories.Commands.CreateCategory;

public record CreateCategoryCommand(CreateCategoryDto CreateCategoryDto) : IRequest<CategoryCreatedDto>;
