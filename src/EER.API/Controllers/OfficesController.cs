﻿using System.Net.Mime;
using EER.Application.Features.Offices.Commands.CreateOffice;
using EER.Application.Features.Offices.Commands.DeleteOffice;
using EER.Application.Features.Offices.Commands.UpdateOffice;
using EER.Application.Features.Offices.Queries.GetAllOffices;
using EER.Application.Features.Offices.Queries.GetOfficeById;
using EER.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EER.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class OfficesController : ControllerBase
{
    private readonly ISender _sender;

    public OfficesController(ISender sender)
    {
        _sender = sender;
    }

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
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var offices = await _sender.Send(new GetAllOfficesQuery(), cancellationToken);
        return Ok(offices);
    }

    // GET: api/offices/1
    /// <summary>
    /// Retrieves a specific office by ID.
    /// </summary>
    /// <param name="id">The ID of the office to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The requested office if found.</returns>
    /// <response code="200">Returns the requested office.</response>
    /// <response code="404">If the office with the specified ID is not found.</response>
    /// <response code="406">The requested content type is not supported.</response>
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [ProducesResponseType(typeof(Office), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var office = await _sender.Send(new GetOfficeByIdQuery(id), cancellationToken);
        return office is not null ? Ok(office) : NotFound();
    }

    // POST: api/offices
    /// <summary>
    /// Creates a new office.
    /// </summary>
    /// <param name="office">The office to create.</param>
    /// <param name="cancellationToken">Cancellation token</param>
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
    public async Task<IActionResult> Create(Office office, CancellationToken cancellationToken)
    {
        var command = new CreateOfficeCommand(
            office.OwnerId,
            office.Address,
            office.City,
            office.Country,
            office.IsActive);

        var createdOffice = await _sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = createdOffice.Id }, createdOffice);
    }

    // PUT: api/offices/1
    /// <summary>
    /// Updates an existing office by ID.
    /// </summary>
    /// <param name="id">The ID of the office to update.</param>
    /// <param name="updatedOffice">The updated office data.</param>
    /// <param name="cancellationToken">Cancellation token</param>
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
    public async Task<IActionResult> Update(int id, Office updatedOffice, CancellationToken cancellationToken)
    {
        var command = new UpdateOfficeCommand(
            id, updatedOffice);

        var office = await _sender.Send(command, cancellationToken);
        return Ok(office);
    }

    // DELETE: api/offices/1
    /// <summary>
    /// Deletes a specific office by ID.
    /// </summary>
    /// <param name="id">The ID of the office to delete.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">The office was successfully deleted.</response>
    /// <response code="404">If the office with the specified ID is not found.</response>
    /// <response code="406">The requested content type is not supported.</response>
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteOfficeCommand(id), cancellationToken);
        return result ? NoContent() : NotFound();
    }
}
