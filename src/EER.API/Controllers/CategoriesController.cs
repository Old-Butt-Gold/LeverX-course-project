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
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_categories.Values);
    }

    // GET: api/categories/1
    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        return _categories.TryGetValue(id, out var category)
            ? Ok(category)
            : NotFound();
    }

    // POST: api/categories
    [HttpPost]
    public IActionResult Create(Category category)
    {
        category.Id = Interlocked.Increment(ref _idCounter);
        category.IsActive = true;
        _categories[category.Id] = category;
        return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
    }

    // PUT: api/categories/1
    [HttpPut("{id:int}")]
    public IActionResult Update(int id, Category updatedCategory)
    {
        if (!_categories.TryGetValue(id, out var category))
        {
            return NoContent();
        }

        category.Name = updatedCategory.Name;
        category.Description = updatedCategory.Description;
        category.IsActive = updatedCategory.IsActive;
        category.Slug = updatedCategory.Slug;
        
        return Ok(category);
    }

    // DELETE: api/categories/1
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        return !_categories.Remove(id) 
            ? NotFound() 
            : NoContent();
    }
}