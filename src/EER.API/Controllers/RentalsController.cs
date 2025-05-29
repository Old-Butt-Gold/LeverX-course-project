using System.Net.Mime;
using EER.Application.Abstractions.Services;
using EER.Domain.Entities;
using EER.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace EER.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class RentalsController : ControllerBase
{
    private readonly IRentalService _rentalService;

    public RentalsController(IRentalService rentalService)
    {
        _rentalService = rentalService;
    }

    // GET: api/rentals
    /// <summary>
    /// Retrieves all rentals.
    /// </summary>
    /// <returns>A list of all rentals.</returns>
    /// <response code="200">Returns the list of rentals.</response>
    /// <response code="406">The requested content type is not supported.</response>
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [ProducesResponseType(typeof(List<Rental>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await _rentalService.GetAllAsync(cancellationToken));
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
    [ProducesResponseType(typeof(Rental), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var rental = await _rentalService.GetByIdAsync(id, cancellationToken);
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
    [ProducesResponseType(typeof(Rental), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpPost]
    public async Task<IActionResult> Create(Rental rental, CancellationToken cancellationToken)
    {
        var createdRental = await _rentalService.CreateAsync(rental, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = createdRental.Id }, createdRental);
    }

    // PUT: api/rentals/1
    /// <summary>
    /// Updates the status of an existing rental by ID.
    /// </summary>
    /// <param name="id">The ID of the rental to update.</param>
    /// <param name="status">The status that rental object will be setting as new status.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated rental.</returns>
    /// <response code="200">Returns the updated rental.</response>
    /// <response code="404">If the rental with the specified ID is not found.</response>
    /// <response code="406">The requested content type is not supported.</response>
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [ProducesResponseType(typeof(Rental), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] RentalStatus status, CancellationToken cancellationToken)
    {
        // TODO updatedBy
        var updatedBy = Guid.NewGuid();
        var rental = await _rentalService.UpdateStatusAsync(id, status, updatedBy, cancellationToken);
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
        return await _rentalService.DeleteAsync(id, cancellationToken)
            ? NoContent()
            : NotFound();
    }
}
