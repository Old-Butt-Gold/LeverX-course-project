using MediatR;

namespace EER.Application.Features.Categories.Queries.GetAllCategories;

public record GetAllCategoriesQuery : IRequest<IEnumerable<CategoryDto>>;
