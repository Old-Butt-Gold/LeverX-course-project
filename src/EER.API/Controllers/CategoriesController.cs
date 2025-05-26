using System.Net.Mime;
using EER.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace EER.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class CategoriesController : ControllerBase
{
    private static readonly Dictionary<int, Category> Categories = [];
    private static int _idCounter;

    // GET: api/categories
    /// <summary>
    /// Retrieves all categories.
    /// </summary>
    /// <returns>A list of all categories.</returns>
    /// <response code="200">Returns the list of categories.</response>
    /// <response code="406">The requested content type is not supported.</response>
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [ProducesResponseType(typeof(IEnumerable<Category>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(Categories.Values.ToList());
    }

    // GET: api/categories/1
    /// <summary>
    /// Retrieves a specific category by ID.
    /// </summary>
    /// <param name="id">The ID of the category to retrieve.</param>
    /// <returns>The requested category if found.</returns>
    /// <response code="200">Returns the requested category.</response>
    /// <response code="404">If the category with the specified ID is not found.</response>
    /// <response code="406">The requested content type is not supported.</response>
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [ProducesResponseType(typeof(Category), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        return Categories.TryGetValue(id, out var category)
            ? Ok(category)
            : NotFound();
    }

    // TODO later use <remarks> for example of DTO and implement BadRequest

    // POST: api/categories
    /// <summary>
    /// Creates a new category.
    /// </summary>
    /// <param name="category">The category to create.</param>
    /// <returns>The id of created category.</returns>
    /// <response code="201">Returns the created category.</response>
    /// <response code="400">If the category data is invalid.</response>
    /// <response code="406">The requested content type is not supported.</response>
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [ProducesResponseType(typeof(Category), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpPost]
    public IActionResult Create(Category category)
    {
        category.Id = Interlocked.Increment(ref _idCounter);
        Categories[category.Id] = category;
        return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
    }

    // PUT: api/categories/1
    /// <summary>
    /// Updates an existing category by ID.
    /// </summary>
    /// <param name="id">The ID of the category to update.</param>
    /// <param name="updatedCategory">The updated category data.</param>
    /// <returns>The updated category.</returns>
    /// <response code="200">Returns the updated category.</response>
    /// <response code="404">If the category with the specified ID is not found.</response>
    /// <response code="406">The requested content type is not supported.</response>
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [ProducesResponseType(typeof(Category), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpPut("{id:int}")]
    public IActionResult Update(int id, Category updatedCategory)
    {
        if (!Categories.TryGetValue(id, out var category))
        {
            return NotFound();
        }

        category.Name = updatedCategory.Name;
        category.Description = updatedCategory.Description;
        category.Slug = updatedCategory.Slug;
        category.TotalEquipment = updatedCategory.TotalEquipment;

        return Ok(category);
    }

    // DELETE: api/categories/1
    /// <summary>
    /// Deletes a specific category by ID.
    /// </summary>
    /// <param name="id">The ID of the category to delete.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">The category was successfully deleted.</response>
    /// <response code="404">If the category with the specified ID is not found.</response>
    /// <response code="406">The requested content type is not supported.</response>
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        return !Categories.Remove(id)
            ? NotFound()
            : NoContent();
    }
}
