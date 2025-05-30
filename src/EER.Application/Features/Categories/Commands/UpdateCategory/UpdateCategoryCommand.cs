using EER.Domain.Entities;
using MediatR;

namespace EER.Application.Features.Categories.Commands.UpdateCategory;

public record UpdateCategoryCommand(int Id, Category Category) : IRequest<Category>;
