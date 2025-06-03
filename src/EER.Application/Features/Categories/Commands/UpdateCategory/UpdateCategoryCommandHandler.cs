using AutoMapper;
using EER.Domain.DatabaseAbstractions;
using EER.Domain.Exceptions;
using MediatR;

namespace EER.Application.Features.Categories.Commands.UpdateCategory;

internal sealed class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, CategoryUpdatedDto>
{
    private readonly ICategoryRepository _repository;
    private readonly IMapper _mapper;

    public UpdateCategoryCommandHandler(ICategoryRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<CategoryUpdatedDto> Handle(UpdateCategoryCommand command, CancellationToken cancellationToken)
    {
        var category = await _repository.GetByIdAsync(command.UpdateCategoryDto.Id, cancellationToken: cancellationToken);

        if (category is null)
            throw new KeyNotFoundException($"Category with ID {command.UpdateCategoryDto.Id} not found");

        _mapper.Map(command.UpdateCategoryDto, category);

        var updatedCategory = await _repository.UpdateAsync(category, cancellationToken: cancellationToken);

        return _mapper.Map<CategoryUpdatedDto>(updatedCategory);
    }
}
