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
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_rentals.Values);
    }

    // GET: api/rentals/1
    [HttpGet("{id:long}")]
    public IActionResult GetById(long id)
    {
        return _rentals.TryGetValue(id, out var rental) 
            ? Ok(rental) 
            : NotFound();
    }

    // POST: api/rentals
    [HttpPost]
    public IActionResult Create(Rental rental)
    {
        rental.Id = Interlocked.Increment(ref _idCounter);
        rental.Status = RentalStatus.Pending;
        _rentals[rental.Id] = rental;
        return CreatedAtAction(nameof(GetById), new { id = rental.Id }, rental);
    }

    // PUT: api/rentals/1
    [HttpPut("{id:long}")]
    public IActionResult Update(long id, Rental updatedRental)
    {
        if (!_rentals.TryGetValue(id, out var rental))
        {
            return NoContent();
        }

        rental.Status = updatedRental.Status;
        return Ok(rental);
    }

    // DELETE: api/rentals/1
    [HttpDelete("{id:long}")]
    public IActionResult Delete(long id)
    {
        return !_rentals.Remove(id) 
            ? NotFound() 
            : NoContent();
    }
}