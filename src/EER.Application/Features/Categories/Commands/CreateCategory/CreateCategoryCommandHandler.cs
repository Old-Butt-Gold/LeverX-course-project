using AutoMapper;
using EER.Domain.DatabaseAbstractions;
using EER.Domain.Entities;
using MediatR;

namespace EER.Application.Features.Categories.Commands.CreateCategory;

internal sealed class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryCreatedDto>
{
    private readonly ICategoryRepository _repository;
    private readonly IMapper _mapper;

    public CreateCategoryCommandHandler(ICategoryRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<CategoryCreatedDto> Handle(CreateCategoryCommand command, CancellationToken cancellationToken)
    {

        var category = _mapper.Map<Category>(command.CreateCategoryDto);
        category.CreatedBy = command.Manipulator;
        category.UpdatedBy = command.Manipulator;

        var createdCategory = await _repository.AddAsync(category, cancellationToken: cancellationToken);

        return _mapper.Map<CategoryCreatedDto>(createdCategory);
    }
}
