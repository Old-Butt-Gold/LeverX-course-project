using System.Net.Mime;
using EER.Domain.Entities;
using EER.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace EER.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class RentalsController : ControllerBase
{
    private static readonly Dictionary<long, Rental> _rentals = [];
    private static long _idCounter;

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
    public IActionResult GetAll()
    {
        return Ok(_rentals.Values.ToList());
    }

    // GET: api/rentals/1
    /// <summary>
    /// Retrieves a specific rental by ID.
    /// </summary>
    /// <param name="id">The ID of the rental to retrieve.</param>
    /// <returns>The requested rental if found.</returns>
    /// <response code="200">Returns the requested rental.</response>
    /// <response code="404">If the rental with the specified ID is not found.</response>
    /// <response code="406">The requested content type is not supported.</response>
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [ProducesResponseType(typeof(Rental), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpGet("{id:long}")]
    public IActionResult GetById(long id)
    {
        return _rentals.TryGetValue(id, out var rental)
            ? Ok(rental)
            : NotFound();
    }

    // POST: api/rentals
    /// <summary>
    /// Creates a new rental.
    /// </summary>
    /// <param name="rental">The rental to create.</param>
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
    public IActionResult Create(Rental rental)
    {
        rental.Id = Interlocked.Increment(ref _idCounter);
        rental.Status = RentalStatus.Pending;
        rental.CreatedAt = DateTime.UtcNow;
        _rentals[rental.Id] = rental;
        return CreatedAtAction(nameof(GetById), new { id = rental.Id }, rental);
    }

    // PUT: api/rentals/1
    /// <summary>
    /// Updates the status of an existing rental by ID.
    /// </summary>
    /// <param name="id">The ID of the rental to update.</param>
    /// <param name="updatedRental">The rental object containing the new status.</param>
    /// <returns>The updated rental.</returns>
    /// <response code="200">Returns the updated rental.</response>
    /// <response code="404">If the rental with the specified ID is not found.</response>
    /// <response code="406">The requested content type is not supported.</response>
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [ProducesResponseType(typeof(Rental), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpPut("{id:long}")]
    public IActionResult Update(long id, Rental updatedRental)
    {
        if (!_rentals.TryGetValue(id, out var rental))
        {
            return NotFound();
        }

        rental.Status = updatedRental.Status;
        return Ok(rental);
    }

    // DELETE: api/rentals/1
    /// <summary>
    /// Deletes a specific rental by ID.
    /// </summary>
    /// <param name="id">The ID of the rental to delete.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">The rental was successfully deleted.</response>
    /// <response code="404">If the rental with the specified ID is not found.</response>
    /// <response code="406">The requested content type is not supported.</response>
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpDelete("{id:long}")]
    public IActionResult Delete(long id)
    {
        return !_rentals.Remove(id)
            ? NotFound()
            : NoContent();
    }
}
