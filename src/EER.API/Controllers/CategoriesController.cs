using EER.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace EER.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class CategoriesController : ControllerBase
{
    private static readonly Dictionary<int, Category> _categories = [];
    private static int _idCounter;
    
    // GET: api/categories
    /// <summary>
    /// Retrieves all categories.
    /// </summary>
    /// <returns>A list of all categories.</returns>
    /// <response code="200">Returns the list of categories.</response>
    [ProducesResponseType(typeof(IEnumerable<Category>), StatusCodes.Status200OK)]
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_categories.Values);
    }

    // GET: api/categories/1
    /// <summary>
    /// Retrieves a specific category by ID.
    /// </summary>
    /// <param name="id">The ID of the category to retrieve.</param>
    /// <returns>The requested category if found.</returns>
    /// <response code="200">Returns the requested category.</response>
    /// <response code="404">If the category with the specified ID is not found.</response>
    [ProducesResponseType(typeof(Category), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        return _categories.TryGetValue(id, out var category)
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
    [ProducesResponseType(typeof(Category), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost]
    public IActionResult Create(Category category)
    {
        category.Id = Interlocked.Increment(ref _idCounter);
        category.IsActive = true;
        _categories[category.Id] = category;
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
    [ProducesResponseType(typeof(Category), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPut("{id:int}")]
    public IActionResult Update(int id, Category updatedCategory)
    {
        if (!_categories.TryGetValue(id, out var category))
        {
            return NotFound();
        }

        category.Name = updatedCategory.Name;
        category.Description = updatedCategory.Description;
        category.IsActive = updatedCategory.IsActive;
        category.Slug = updatedCategory.Slug;
        
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
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        return !_categories.Remove(id) 
            ? NotFound() 
            : NoContent();
    }
}