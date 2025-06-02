using System.Net.Mime;
using EER.Application.Features.Rentals.Commands.CreateRental;
using EER.Application.Features.Rentals.Commands.DeleteRental;
using EER.Application.Features.Rentals.Commands.UpdateRentalStatus;
using EER.Application.Features.Rentals.Queries.GetAllRentals;
using EER.Application.Features.Rentals.Queries.GetRentalById;
using EER.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EER.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class RentalsController : ControllerBase
{
    private readonly ISender _sender;

    public RentalsController(ISender sender)
    {
        _sender = sender;
    }

    // GET: api/rentals
    /// <summary>
    /// Retrieves all rentals.
    /// </summary>
    /// <returns>A list of all rentals.</returns>
    /// <response code="200">Returns the list of rentals.</response>
    /// <response code="406">The requested content type is not supported.</response>
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [ProducesResponseType(typeof(IEnumerable<RentalDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var rentals = await _sender.Send(new GetAllRentalsQuery(), cancellationToken);
        return Ok(rentals);
    }

    // GET: api/rentals/1
    /// <summary>
    /// Retrieves a specific rental by ID.
    /// </summary>
    /// <param name="id">The ID of the rental to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The requested rental if found.</returns>
    /// <response code="200">Returns the requested rental.</response>
    /// <response code="404">If the rental with the specified ID is not found.</response>
    /// <response code="406">The requested content type is not supported.</response>
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [ProducesResponseType(typeof(RentalDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var rental = await _sender.Send(new GetRentalByIdQuery(id), cancellationToken);
        return rental is not null ? Ok(rental) : NotFound();
    }

    // POST: api/rentals
    /// <summary>
    /// Creates a new rental.
    /// </summary>
    /// <param name="rental">The rental to create.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created rental.</returns>
    /// <response code="201">Returns the created rental ID.</response>
    /// <response code="400">If the rental data is invalid.</response>
    /// <response code="406">The requested content type is not supported.</response>
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [ProducesResponseType(typeof(RentalCreatedDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpPost]
    public async Task<IActionResult> Create(CreateRentalDto rental, CancellationToken cancellationToken)
    {
        var command = new CreateRentalCommand(rental);

        var createdRental = await _sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = createdRental.Id }, createdRental);
    }

    // PUT: api/rentals/1
    /// <summary>
    /// Updates the status of an existing rental by ID.
    /// </summary>
    /// <param name="rentalDto">The dto that required ID of rental and RentalStatus to change as new status.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated rental.</returns>
    /// <response code="200">Returns the updated rental.</response>
    /// <response code="404">If the rental with the specified ID is not found.</response>
    /// <response code="406">The requested content type is not supported.</response>
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [ProducesResponseType(typeof(RentalUpdatedDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateRentalDto rentalDto, CancellationToken cancellationToken)
    {
        var command = new UpdateRentalStatusCommand(rentalDto);

        var rental = await _sender.Send(command, cancellationToken);
        return Ok(rental);
    }

    // DELETE: api/rentals/1
    /// <summary>
    /// Deletes a specific rental by ID.
    /// </summary>
    /// <param name="id">The ID of the rental to delete.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">The rental was successfully deleted.</response>
    /// <response code="404">If the rental with the specified ID is not found.</response>
    /// <response code="406">The requested content type is not supported.</response>
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteRentalCommand(id), cancellationToken);
        return result ? NoContent() : NotFound();
    }
}
