using System.Net.Mime;
using EER.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace EER.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class EquipmentController : ControllerBase
{
    private static readonly Dictionary<long, Equipment> _equipment = [];
    private static long _idCounter;

    // GET: api/equipment
    /// <summary>
    /// Retrieves all equipment items.
    /// </summary>
    /// <returns>A list of all equipment items.</returns>
    /// <response code="200">Returns the list of equipment items.</response>
    /// <response code="406">The requested content type is not supported.</response>
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [ProducesResponseType(typeof(List<Equipment>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_equipment.Values.ToList());
    }

    // GET: api/equipment/1
    /// <summary>
    /// Retrieves a specific equipment item by ID.
    /// </summary>
    /// <param name="id">The ID of the equipment to retrieve.</param>
    /// <returns>The requested equipment if found.</returns>
    /// <response code="200">Returns the requested equipment.</response>
    /// <response code="404">If the equipment with the specified ID is not found.</response>
    /// <response code="406">The requested content type is not supported.</response>
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [ProducesResponseType(typeof(Equipment), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpGet("{id:long}")]
    public IActionResult GetById(long id)
    {
        return _equipment.TryGetValue(id, out var item) 
            ? Ok(item) 
            : NotFound();
    }

    // GET: api/equipment/category/1
    /// <summary>
    /// Retrieves all equipment items by category ID.
    /// </summary>
    /// <param name="categoryId">The ID of the category to filter by.</param>
    /// <returns>A list of equipment items in the specified category.</returns>
    /// <response code="200">Returns the list of equipment items in the category.</response>
    /// <response code="404">If no equipment is found for the specified category.</response>
    /// <response code="406">The requested content type is not supported.</response>
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [ProducesResponseType(typeof(List<Equipment>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpGet("category/{categoryId:int}")]
    public IActionResult GetByCategory(int categoryId)
    {
        var items = _equipment.Values
            .Where(e => e.CategoryId == categoryId)
            .ToList();
        
        return items.Count > 0
            ? Ok(items) 
            : NotFound();
    }

    // POST: api/equipment
    /// <summary>
    /// Creates a new equipment item.
    /// </summary>
    /// <param name="equipment">The equipment item to create.</param>
    /// <returns>The created equipment item.</returns>
    /// <response code="201">Returns the created equipment item ID.</response>
    /// <response code="400">If the equipment data is invalid.</response>
    /// <response code="406">The requested content type is not supported.</response>
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [ProducesResponseType(typeof(Equipment), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpPost]
    public IActionResult Create(Equipment equipment)
    {
        equipment.Id = Interlocked.Increment(ref _idCounter);
        equipment.UpdatedAt = DateTime.UtcNow;
        _equipment[equipment.Id] = equipment;
        return CreatedAtAction(nameof(GetById), new { id = equipment.Id }, equipment);
    }

    // PUT: api/equipment/1
    /// <summary>
    /// Updates an existing equipment item by ID.
    /// </summary>
    /// <param name="id">The ID of the equipment to update.</param>
    /// <param name="updatedEquipment">The updated equipment data.</param>
    /// <returns>The updated equipment item.</returns>
    /// <response code="200">Returns the updated equipment item.</response>
    /// <response code="404">If the equipment with the specified ID is not found.</response>
    /// <response code="406">The requested content type is not supported.</response>
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [ProducesResponseType(typeof(Equipment), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpPut("{id:long}")]
    public IActionResult Update(long id, Equipment updatedEquipment)
    {
        if (!_equipment.TryGetValue(id, out var equipment))
        {
            return NotFound();
        }

        equipment.Name = updatedEquipment.Name;
        equipment.Location = updatedEquipment.Location;
        equipment.CategoryId = updatedEquipment.CategoryId;
        equipment.AvailableQuantity = updatedEquipment.AvailableQuantity;
        equipment.Description = updatedEquipment.Description;
        equipment.PricePerDay = updatedEquipment.PricePerDay;
        equipment.UpdatedAt = DateTime.UtcNow;
        return Ok(equipment);
    }

    // DELETE: api/equipment/1
    /// <summary>
    /// Deletes a specific equipment item by ID.
    /// </summary>
    /// <param name="id">The ID of the equipment to delete.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">The equipment was successfully deleted.</response>
    /// <response code="404">If the equipment with the specified ID is not found.</response>
    /// <response code="406">The requested content type is not supported.</response>
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpDelete("{id:long}")]
    public IActionResult Delete(long id)
    {
        return !_equipment.Remove(id) 
            ? NotFound() 
            : NoContent();
    }
}