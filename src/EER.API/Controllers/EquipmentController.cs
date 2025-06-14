﻿using System.Net.Mime;
using EER.API.Constants;
using EER.Application.Extensions;
using EER.Application.Features.Equipment.Commands.CreateEquipment;
using EER.Application.Features.Equipment.Commands.DeleteEquipment;
using EER.Application.Features.Equipment.Commands.ModerateEquipment;
using EER.Application.Features.Equipment.Commands.UpdateEquipment;
using EER.Application.Features.Equipment.Queries.GetAllEquipment;
using EER.Application.Features.Equipment.Queries.GetEquipmentById;
using EER.Application.Features.Equipment.Queries.GetUnmoderatedEquipment;
using EER.Application.Features.Reviews.Commands.CreateReview;
using EER.Application.Features.Reviews.Commands.DeleteReview;
using EER.Application.Features.Reviews.Queries.GetReviewsByEquipmentId;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace EER.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = AuthRoleConstants.AnyRole)]
[EnableRateLimiting(RateLimiterConstants.PerUser)]
public sealed class EquipmentController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ILogger<EquipmentController> _logger;

    public EquipmentController(ISender sender, ILogger<EquipmentController> logger)
    {
        _sender = sender;
        _logger = logger;
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
    [AllowAnonymous]
    [EnableRateLimiting(RateLimiterConstants.PerIp)]
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
    [AllowAnonymous]
    [EnableRateLimiting(RateLimiterConstants.PerIp)]
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
    [Authorize(Policy = AuthRoleConstants.OwnerOnly)]
    public async Task<IActionResult> Create(CreateEquipmentDto equipment, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        _logger.LogInformation("User {UserId} creating new equipment: {@EquipmentData}", userId, equipment);

        var command = new CreateEquipmentCommand(equipment, userId);
        var createdItem = await _sender.Send(command, cancellationToken);

        _logger.LogInformation("User {UserId} created equipment ID: {EquipmentId}", userId, createdItem.Id);
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
    [Authorize(Policy = AuthRoleConstants.OwnerOnly)]
    public async Task<IActionResult> Update(UpdateEquipmentDto updatedEquipment, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        _logger.LogInformation("User {UserId} updating equipment ID: {EquipmentId}", userId, updatedEquipment.Id);

        var command = new UpdateEquipmentCommand(updatedEquipment, userId);
        var result = await _sender.Send(command, cancellationToken);

        _logger.LogInformation("User {UserId} updated equipment ID: {EquipmentId}", userId, updatedEquipment.Id);
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
    [Authorize(Policy = AuthRoleConstants.OwnerOrAdmin)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var role = User.GetRole();
        _logger.LogInformation("User {UserId} deleting equipment ID: {EquipmentId}", userId, id);
        var dto = new DeleteEquipmentRequestDto { Id = id, Manipulator = userId, UserRole = role };

        var command = new DeleteEquipmentCommand(dto);

        var result = await _sender.Send(command, cancellationToken);

        return result ? NoContent() : NotFound();
    }

    // GET: api/equipment/unmoderated
    [HttpGet("unmoderated")]
    [Authorize(Policy = AuthRoleConstants.AdminOnly)]
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [ProducesResponseType(typeof(IEnumerable<EquipmentForModerationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    public async Task<IActionResult> GetUnmoderated(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Admin {UserId} requested unmoderated equipment", User.GetUserId());

        var equipment = await _sender.Send(new GetUnmoderatedEquipmentQuery(), cancellationToken);

        return Ok(equipment);
    }

    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpPatch("{id:int}/moderate")]
    [Authorize(Policy = AuthRoleConstants.AdminOnly)]
    public async Task<IActionResult> Moderate(int id, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        _logger.LogInformation("Admin {UserId} moderating equipment ID: {EquipmentId}", userId, id);

        var result = await _sender.Send(new ModerateEquipmentCommand(id, userId), cancellationToken);

        if (result)
        {
            _logger.LogInformation("Admin {UserId} moderated equipment ID: {EquipmentId} successfully", userId, id);
            return NoContent();
        }

        _logger.LogWarning("Admin {UserId} failed to moderate equipment ID: {EquipmentId} (not found)", userId, id);
        return NotFound();
    }

    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ReviewCreatedDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPost("{equipmentId:int}/reviews")]
    [Authorize(Policy = AuthRoleConstants.CustomerOnly)]
    public async Task<IActionResult> CreateReview(int equipmentId, [FromBody] CreateReviewDto reviewDto, CancellationToken cancellationToken)
    {
        var customerId = User.GetUserId();
        _logger.LogInformation("User {UserId} adding review to equipment ID: {EquipmentId}", customerId, equipmentId);

        var command = new CreateReviewCommand(reviewDto, equipmentId, customerId);
        var result = await _sender.Send(command, cancellationToken);

        _logger.LogInformation("User {UserId} added review to equipment ID: {EquipmentId}", customerId, equipmentId);
        return Ok(result);
    }

    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [ProducesResponseType(typeof(IEnumerable<ReviewWithUserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpGet("{equipmentId:int}/reviews")]
    [AllowAnonymous]
    [EnableRateLimiting(RateLimiterConstants.PerIp)]
    public async Task<IActionResult> GetReviews(int equipmentId, CancellationToken cancellationToken)
    {
        var reviews = await _sender.Send(new GetReviewsByEquipmentIdQuery(equipmentId), cancellationToken);

        return Ok(reviews);
    }

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpDelete("{equipmentId:int}/reviews")]
    [Authorize(Policy = AuthRoleConstants.CustomerOnly)]
    public async Task<IActionResult> DeleteReview(int equipmentId, CancellationToken cancellationToken)
    {
        var customerId = User.GetUserId();

        var command = new DeleteReviewCommand(equipmentId, customerId);
        var result = await _sender.Send(command, cancellationToken);

        return result ? NoContent() : NotFound();
    }
}
