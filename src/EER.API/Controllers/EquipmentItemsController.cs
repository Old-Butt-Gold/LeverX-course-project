using System.Net.Mime;
using EER.Application.Extensions;
using EER.Application.Features.EquipmentItems.Commands.CreateEquipmentItem;
using EER.Application.Features.EquipmentItems.Commands.DeleteEquipmentItem;
using EER.Application.Features.EquipmentItems.Commands.UpdateEquipmentItem;
using EER.Application.Features.EquipmentItems.Queries.GetAllEquipmentItems;
using EER.Application.Features.EquipmentItems.Queries.GetEquipmentItemById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EER.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public sealed class EquipmentItemsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ILogger<EquipmentItemsController> _logger;

    public EquipmentItemsController(ISender sender, ILogger<EquipmentItemsController> logger)
    {
        _sender = sender;
        _logger = logger;
    }

    // GET: api/equipmentitems
    /// <summary>
    /// Retrieves all equipment items.
    /// </summary>
    /// <returns>A list of all equipment items.</returns>
    /// <response code="200">Returns the list of equipment items.</response>
    /// <response code="406">The requested content type is not supported.</response>
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [ProducesResponseType(typeof(IEnumerable<EquipmentItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        _logger.LogInformation("User {UserId} requested all equipment items", User.GetUserId());
        var items = await _sender.Send(new GetAllEquipmentItemsQuery(), cancellationToken);
        return Ok(items);
    }

    // GET: api/equipmentitems/1
    /// <summary>
    /// Retrieves a specific equipment item by ID.
    /// </summary>
    /// <param name="id">The ID of the equipment item to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The requested equipment item if found.</returns>
    /// <response code="200">Returns the requested equipment item.</response>
    /// <response code="404">If the equipment item with the specified ID is not found.</response>
    /// <response code="406">The requested content type is not supported.</response>
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [ProducesResponseType(typeof(EquipmentItemDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("User {UserId} requested equipment item ID: {ItemId}", User.GetUserId(), id);
        var item = await _sender.Send(new GetEquipmentItemByIdQuery(id), cancellationToken);
        return item is not null ? Ok(item) : NotFound();
    }

    // POST: api/equipmentitems
    /// <summary>
    /// Creates a new equipment item.
    /// </summary>
    /// <param name="item">The equipment item to create.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created equipment item.</returns>
    /// <response code="201">Returns the created equipment item ID.</response>
    /// <response code="400">If the equipment item data is invalid.</response>
    /// <response code="406">The requested content type is not supported.</response>
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [ProducesResponseType(typeof(EquipmentItemCreatedDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpPost]
    public async Task<IActionResult> Create(CreateEquipmentItemDto item, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        _logger.LogInformation("User {UserId} creating new equipment item: {@ItemData}", userId, item);

        var command = new CreateEquipmentItemCommand(item, userId);
        var createdItem = await _sender.Send(command, cancellationToken);

        _logger.LogInformation("User {UserId} created equipment item ID: {ItemId}", userId, createdItem.Id);
        return CreatedAtAction(nameof(GetById), new { id = createdItem.Id }, createdItem);
    }

    // PUT: api/equipmentitems/1
    /// <summary>
    /// Updates an existing equipment item by ID.
    /// </summary>
    /// <param name="updatedItem">The updated equipment item data.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated equipment item.</returns>
    /// <response code="200">Returns the updated equipment item.</response>
    /// <response code="404">If the equipment item with the specified ID is not found.</response>
    /// <response code="406">The requested content type is not supported.</response>
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [ProducesResponseType(typeof(EquipmentItemUpdatedDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpPut]
    public async Task<IActionResult> Update(UpdateEquipmentItemDto updatedItem, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        _logger.LogInformation("User {UserId} updating equipment item ID: {ItemId}", userId, updatedItem.Id);

        var command = new UpdateEquipmentItemCommand(updatedItem, userId);
        var item = await _sender.Send(command, cancellationToken);

        _logger.LogInformation("User {UserId} updated equipment item ID: {ItemId}", userId, updatedItem.Id);
        return Ok(item);
    }

    // DELETE: api/equipmentitems/1
    /// <summary>
    /// Deletes a specific equipment item by ID.
    /// </summary>
    /// <param name="id">The ID of the equipment item to delete.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">The equipment item was successfully deleted.</response>
    /// <response code="404">If the equipment item with the specified ID is not found.</response>
    /// <response code="406">The requested content type is not supported.</response>
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        _logger.LogInformation("User {UserId} deleting equipment item ID: {ItemId}", userId, id);

        var result = await _sender.Send(new DeleteEquipmentItemCommand(id), cancellationToken);
        return result ? NoContent() : NotFound();
    }
}
