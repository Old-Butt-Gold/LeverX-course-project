using System.Net.Mime;
using EER.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace EER.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class EquipmentItemsController : ControllerBase
{
    private static readonly Dictionary<long, EquipmentItem> EquipmentItems = new();
    private static long _idCounter;

    // GET: api/equipmentitems
    /// <summary>
    /// Retrieves all equipment items.
    /// </summary>
    /// <returns>A list of all equipment items.</returns>
    /// <response code="200">Returns the list of equipment items.</response>
    /// <response code="406">The requested content type is not supported.</response>
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [ProducesResponseType(typeof(List<EquipmentItem>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(EquipmentItems.Values.ToList());
    }

    // GET: api/equipmentitems/1
    /// <summary>
    /// Retrieves a specific equipment item by ID.
    /// </summary>
    /// <param name="id">The ID of the equipment item to retrieve.</param>
    /// <returns>The requested equipment item if found.</returns>
    /// <response code="200">Returns the requested equipment item.</response>
    /// <response code="404">If the equipment item with the specified ID is not found.</response>
    /// <response code="406">The requested content type is not supported.</response>
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [ProducesResponseType(typeof(EquipmentItem), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpGet("{id:long}")]
    public IActionResult GetById(long id)
    {
        return EquipmentItems.TryGetValue(id, out var item)
            ? Ok(item)
            : NotFound();
    }

    // POST: api/equipmentitems
    /// <summary>
    /// Creates a new equipment item.
    /// </summary>
    /// <param name="item">The equipment item to create.</param>
    /// <returns>The created equipment item.</returns>
    /// <response code="201">Returns the created equipment item ID.</response>
    /// <response code="400">If the equipment item data is invalid.</response>
    /// <response code="406">The requested content type is not supported.</response>
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [ProducesResponseType(typeof(EquipmentItem), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpPost]
    public IActionResult Create(EquipmentItem item)
    {
        item.Id = Interlocked.Increment(ref _idCounter);
        EquipmentItems[item.Id] = item;
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    // PUT: api/equipmentitems/1
    /// <summary>
    /// Updates an existing equipment item by ID.
    /// </summary>
    /// <param name="id">The ID of the equipment item to update.</param>
    /// <param name="updatedItem">The updated equipment item data.</param>
    /// <returns>The updated equipment item.</returns>
    /// <response code="200">Returns the updated equipment item.</response>
    /// <response code="404">If the equipment item with the specified ID is not found.</response>
    /// <response code="406">The requested content type is not supported.</response>
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [ProducesResponseType(typeof(EquipmentItem), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpPut("{id:long}")]
    public IActionResult Update(long id, EquipmentItem updatedItem)
    {
        if (!EquipmentItems.TryGetValue(id, out var item))
        {
            return NotFound();
        }

        item.EquipmentId = updatedItem.EquipmentId;
        item.OfficeId = updatedItem.OfficeId;
        item.SerialNumber = updatedItem.SerialNumber;
        item.ItemStatus = updatedItem.ItemStatus;
        item.MaintenanceDate = updatedItem.MaintenanceDate;
        item.PurchaseDate = updatedItem.PurchaseDate;

        return Ok(item);
    }

    // DELETE: api/equipmentitems/1
    /// <summary>
    /// Deletes a specific equipment item by ID.
    /// </summary>
    /// <param name="id">The ID of the equipment item to delete.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">The equipment item was successfully deleted.</response>
    /// <response code="404">If the equipment item with the specified ID is not found.</response>
    /// <response code="406">The requested content type is not supported.</response>
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpDelete("{id:long}")]
    public IActionResult Delete(long id)
    {
        return !EquipmentItems.Remove(id)
            ? NotFound()
            : NoContent();
    }
}
