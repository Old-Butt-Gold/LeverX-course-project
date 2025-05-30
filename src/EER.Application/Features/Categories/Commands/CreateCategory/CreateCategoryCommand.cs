using EER.Domain.Entities;
using MediatR;

namespace EER.Application.Features.Categories.Commands.CreateCategory;

public record CreateCategoryCommand(string Name,
    string Description,
    string Slug) : IRequest<Category>;
