using EER.Domain.Entities;
using EER.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace EER.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RentalsController : ControllerBase
{
    private static readonly Dictionary<long, Rental> _rentals = new();
    private static long _idCounter;

    // GET: api/rentals
    /// <summary>
    /// Retrieves all rentals.
    /// </summary>
    /// <returns>A list of all rentals.</returns>
    /// <response code="200">Returns the list of rentals.</response>
    [ProducesResponseType(typeof(IEnumerable<Rental>), StatusCodes.Status200OK)]
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_rentals.Values);
    }

    // GET: api/rentals/1
    /// <summary>
    /// Retrieves a specific rental by ID.
    /// </summary>
    /// <param name="id">The ID of the rental to retrieve.</param>
    /// <returns>The requested rental if found.</returns>
    /// <response code="200">Returns the requested rental.</response>
    /// <response code="404">If the rental with the specified ID is not found.</response>
    [ProducesResponseType(typeof(Rental), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
    [ProducesResponseType(typeof(Rental), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost]
    public IActionResult Create(Rental rental)
    {
        rental.Id = Interlocked.Increment(ref _idCounter);
        rental.Status = RentalStatus.Pending;
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
    [ProducesResponseType(typeof(Rental), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpDelete("{id:long}")]
    public IActionResult Delete(long id)
    {
        return !_rentals.Remove(id) 
            ? NotFound() 
            : NoContent();
    }
}