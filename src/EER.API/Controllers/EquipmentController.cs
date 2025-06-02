using System.Net.Mime;
using EER.Application.Features.Equipment.Commands.CreateEquipment;
using EER.Application.Features.Equipment.Commands.DeleteEquipment;
using EER.Application.Features.Equipment.Commands.UpdateEquipment;
using EER.Application.Features.Equipment.Queries.GetAllEquipment;
using EER.Application.Features.Equipment.Queries.GetEquipmentById;
using EER.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EER.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class EquipmentController : ControllerBase
{
    private readonly ISender _sender;

    public EquipmentController(ISender sender)
    {
        _sender = sender;
    }


    // GET: api/equipment
    /// <summary>
    /// Retrieves all equipment items.
    /// </summary>
    /// <returns>A list of all equipment items.</returns>
    /// <response code="200">Returns the list of equipment items.</response>
    /// <response code="406">The requested content type is not supported.</response>
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [ProducesResponseType(typeof(IEnumerable<EquipmentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var equipment = await _sender.Send(new GetAllEquipmentQuery(), cancellationToken);
        return Ok(equipment);
    }

    // GET: api/equipment/1
    /// <summary>
    /// Retrieves a specific equipment item by ID.
    /// </summary>
    /// <param name="id">The ID of the equipment to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The requested equipment if found.</returns>
    /// <response code="200">Returns the requested equipment.</response>
    /// <response code="404">If the equipment with the specified ID is not found.</response>
    /// <response code="406">The requested content type is not supported.</response>
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [ProducesResponseType(typeof(EquipmentDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var item = await _sender.Send(new GetEquipmentByIdQuery(id), cancellationToken);
        return item is not null ? Ok(item) : NotFound();
    }

    // POST: api/equipment
    /// <summary>
    /// Creates a new equipment item.
    /// </summary>
    /// <param name="equipment">The equipment item to create.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created equipment item.</returns>
    /// <response code="201">Returns the created equipment item ID.</response>
    /// <response code="400">If the equipment data is invalid.</response>
    /// <response code="406">The requested content type is not supported.</response>
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [ProducesResponseType(typeof(EquipmentCreatedDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpPost]
    public async Task<IActionResult> Create(CreateEquipmentDto equipment, CancellationToken cancellationToken)
    {
        var command = new CreateEquipmentCommand(equipment);
        var createdItem = await _sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = createdItem.Id }, createdItem);
    }

    // PUT: api/equipment/1
    /// <summary>
    /// Updates an existing equipment item by ID.
    /// </summary>
    /// <param name="updatedEquipment">The updated equipment data.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The updated equipment item.</returns>
    /// <response code="200">Returns the updated equipment item.</response>
    /// <response code="404">If the equipment with the specified ID is not found.</response>
    /// <response code="406">The requested content type is not supported.</response>
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [ProducesResponseType(typeof(EquipmentUpdatedDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpPut]
    public async Task<IActionResult> Update(UpdateEquipmentDto updatedEquipment, CancellationToken cancellationToken)
    {
        var command = new UpdateEquipmentCommand(updatedEquipment);
        var result = await _sender.Send(command, cancellationToken);
        return Ok(result);
    }

    // DELETE: api/equipment/1
    /// <summary>
    /// Deletes a specific equipment item by ID.
    /// </summary>
    /// <param name="id">The ID of the equipment to delete.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">The equipment was successfully deleted.</response>
    /// <response code="404">If the equipment with the specified ID is not found.</response>
    /// <response code="406">The requested content type is not supported.</response>
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteEquipmentCommand(id), cancellationToken);
        return result ? NoContent() : NotFound();
    }
}
