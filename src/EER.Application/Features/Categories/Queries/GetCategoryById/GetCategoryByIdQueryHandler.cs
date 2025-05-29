using EER.Domain.DatabaseAbstractions;
using EER.Domain.Entities;
using MediatR;

namespace EER.Application.Features.Categories.Queries.GetCategoryById;

public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, Category?>
{
    private readonly ICategoryRepository _repository;

    public GetCategoryByIdQueryHandler(ICategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<Category?> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync(request.Id, cancellationToken);
    }
}
