using System.Net.Mime;
using EER.API.Constants;
using EER.Application.Extensions;
using EER.Application.Features.Categories.Commands.CreateCategory;
using EER.Application.Features.Categories.Commands.DeleteCategory;
using EER.Application.Features.Categories.Commands.UpdateCategory;
using EER.Application.Features.Categories.Queries.GetAllCategories;
using EER.Application.Features.Categories.Queries.GetCategoryById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace EER.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = AuthRoleConstants.AdminOnly)]
[EnableRateLimiting(RateLimiterConstants.PerUser)]
public sealed class CategoriesController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(ISender sender, ILogger<CategoriesController> logger)
    {
        _sender = sender;
        _logger = logger;
    }

    // GET: api/categories
    /// <summary>
    /// Retrieves all categories.
    /// </summary>
    /// <returns>A list of all categories.</returns>
    /// <response code="200">Returns the list of categories.</response>
    /// <response code="406">The requested content type is not supported.</response>
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [ProducesResponseType(typeof(IEnumerable<CategoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpGet]
    [AllowAnonymous]
    [EnableRateLimiting(RateLimiterConstants.PerIp)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var categories = await _sender.Send(new GetAllCategoriesQuery(), cancellationToken);
        return Ok(categories);
    }

    // GET: api/categories/1
    /// <summary>
    /// Retrieves a specific category by ID.
    /// </summary>
    /// <param name="id">The ID of the category to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The requested category if found.</returns>
    /// <response code="200">Returns the requested category.</response>
    /// <response code="404">If the category with the specified ID is not found.</response>
    /// <response code="406">The requested content type is not supported.</response>
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [ProducesResponseType(typeof(CategoryDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    [EnableRateLimiting(RateLimiterConstants.PerIp)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var category = await _sender.Send(new GetCategoryByIdQuery(id), cancellationToken);
        return category is not null ? Ok(category) : NotFound();
    }

    // POST: api/categories
    /// <summary>
    /// Creates a new category.
    /// </summary>
    /// <param name="category">The category to create.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The id of created category.</returns>
    /// <response code="201">Returns the created category.</response>
    /// <response code="400">If the category data is invalid.</response>
    /// <response code="406">The requested content type is not supported.</response>
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [ProducesResponseType(typeof(CategoryCreatedDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoryDto category, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        _logger.LogInformation("User {UserId} creating new category: {@CategoryData}", userId, category);
        var command = new CreateCategoryCommand(category, userId);

        var createdCategory = await _sender.Send(command, cancellationToken);
        _logger.LogInformation("User {UserId} created category ID: {CategoryId}", userId, createdCategory.Id);

        return CreatedAtAction(nameof(GetById), new { id = createdCategory.Id }, createdCategory);
    }

    // PUT: api/categories/1
    /// <summary>
    /// Updates an existing category by ID.
    /// </summary>
    /// <param name="updatedCategory">The updated category data.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated category.</returns>
    /// <response code="200">Returns the updated category.</response>
    /// <response code="404">If the category with the specified ID is not found.</response>
    /// <response code="406">The requested content type is not supported.</response>
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [ProducesResponseType(typeof(CategoryUpdatedDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpPut]
    public async Task<IActionResult> Update(UpdateCategoryDto updatedCategory, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        _logger.LogInformation("User {UserId} updating category ID: {CategoryId}", userId, updatedCategory.Id);

        var command = new UpdateCategoryCommand(updatedCategory, userId);
        var category = await _sender.Send(command, cancellationToken);

        _logger.LogInformation("User {UserId} updated category ID: {CategoryId}", userId, updatedCategory.Id);
        return Ok(category);
    }

    // DELETE: api/categories/1
    /// <summary>
    /// Deletes a specific category by ID.
    /// </summary>
    /// <param name="id">The ID of the category to delete.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">The category was successfully deleted.</response>
    /// <response code="404">If the category with the specified ID is not found.</response>
    /// <response code="406">The requested content type is not supported.</response>
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        _logger.LogInformation("User {UserId} deleting category ID: {CategoryId}", userId, id);

        var result = await _sender.Send(new DeleteCategoryCommand(id), cancellationToken);
        return result ? NoContent() : NotFound();
    }
}
