using System.Net.Mime;
using EER.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace EER.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class OfficesController : ControllerBase
{
    private static readonly Dictionary<int, Office> Offices = new();
    private static int _idCounter;

    // GET: api/offices
    /// <summary>
    /// Retrieves all offices.
    /// </summary>
    /// <returns>A list of all offices.</returns>
    /// <response code="200">Returns the list of offices.</response>
    /// <response code="406">The requested content type is not supported.</response>
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [ProducesResponseType(typeof(List<Office>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(Offices.Values.ToList());
    }

    // GET: api/offices/1
    /// <summary>
    /// Retrieves a specific office by ID.
    /// </summary>
    /// <param name="id">The ID of the office to retrieve.</param>
    /// <returns>The requested office if found.</returns>
    /// <response code="200">Returns the requested office.</response>
    /// <response code="404">If the office with the specified ID is not found.</response>
    /// <response code="406">The requested content type is not supported.</response>
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [ProducesResponseType(typeof(Office), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        return Offices.TryGetValue(id, out var office)
            ? Ok(office)
            : NotFound();
    }

    // POST: api/offices
    /// <summary>
    /// Creates a new office.
    /// </summary>
    /// <param name="office">The office to create.</param>
    /// <returns>The created office.</returns>
    /// <response code="201">Returns the created office ID.</response>
    /// <response code="400">If the office data is invalid.</response>
    /// <response code="406">The requested content type is not supported.</response>
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [ProducesResponseType(typeof(Office), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpPost]
    public IActionResult Create(Office office)
    {
        office.Id = Interlocked.Increment(ref _idCounter);
        Offices[office.Id] = office;
        return CreatedAtAction(nameof(GetById), new { id = office.Id }, office);
    }

    // PUT: api/offices/1
    /// <summary>
    /// Updates an existing office by ID.
    /// </summary>
    /// <param name="id">The ID of the office to update.</param>
    /// <param name="updatedOffice">The updated office data.</param>
    /// <returns>The updated office.</returns>
    /// <response code="200">Returns the updated office.</response>
    /// <response code="404">If the office with the specified ID is not found.</response>
    /// <response code="406">The requested content type is not supported.</response>
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [ProducesResponseType(typeof(Office), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpPut("{id:int}")]
    public IActionResult Update(int id, Office updatedOffice)
    {
        if (!Offices.TryGetValue(id, out var office))
        {
            return NotFound();
        }

        office.OwnerId = updatedOffice.OwnerId;
        office.Address = updatedOffice.Address;
        office.City = updatedOffice.City;
        office.Country = updatedOffice.Country;
        office.IsActive = updatedOffice.IsActive;

        return Ok(office);
    }

    // DELETE: api/offices/1
    /// <summary>
    /// Deletes a specific office by ID.
    /// </summary>
    /// <param name="id">The ID of the office to delete.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">The office was successfully deleted.</response>
    /// <response code="404">If the office with the specified ID is not found.</response>
    /// <response code="406">The requested content type is not supported.</response>
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        return !Offices.Remove(id)
            ? NotFound()
            : NoContent();
    }
}
