using EER.Domain.Entities;
using MediatR;

namespace EER.Application.Features.Categories.Queries.GetCategoryById;

public record GetCategoryByIdQuery(int Id) : IRequest<Category?>;
