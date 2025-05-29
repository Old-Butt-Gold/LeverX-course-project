using System.Net.Mime;
using EER.Application.Abstractions.Services;
using EER.Application.Features.Users.Commands.CreateUser;
using EER.Application.Features.Users.Commands.DeleteUser;
using EER.Application.Features.Users.Commands.UpdateUser;
using EER.Application.Features.Users.Queries.GetAllUsers;
using EER.Application.Features.Users.Queries.GetUserById;
using EER.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EER.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class UsersController : ControllerBase
{
    private readonly ISender _sender;

    public UsersController(ISender sender)
    {
        _sender = sender;
    }

    // GET: api/users
    /// <summary>
    /// Retrieves all users.
    /// </summary>
    /// <returns>A list of all users.</returns>
    /// <response code="200">Returns the list of users.</response>
    /// <response code="406">The requested content type is not supported.</response>
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [ProducesResponseType(typeof(List<User>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var users = await _sender.Send(new GetAllUsersQuery(), cancellationToken);
        return Ok(users);
    }

    // GET: api/users/1
    /// <summary>
    /// Retrieves a specific user by ID.
    /// </summary>
    /// <param name="id">The ID of the user to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The requested user if found.</returns>
    /// <response code="200">Returns the requested user.</response>
    /// <response code="404">If the user with the specified ID is not found.</response>
    /// <response code="406">The requested content type is not supported.</response>
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var user = await _sender.Send(new GetUserByIdQuery(id), cancellationToken);
        return user is not null ? Ok(user) : NotFound();
    }

    // POST: api/users
    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="user">The user to create. Note: PasswordHash should contain the plain password, which will be hashed by the server.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created user.</returns>
    /// <response code="201">Returns the created user ID.</response>
    /// <response code="400">If the user data is invalid.</response>
    /// <response code="406">The requested content type is not supported.</response>
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [ProducesResponseType(typeof(User), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpPost]
    public async Task<IActionResult> Create(User user, CancellationToken cancellationToken)
    {
        var command = new CreateUserCommand(
            user.Email,
            user.FullName,
            user.PasswordHash,
            user.UserRole);

        var createdUser = await _sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = createdUser.Id }, createdUser);
    }

    // PUT: api/users/1
    /// <summary>
    /// Updates an existing user by ID.
    /// </summary>
    /// <param name="id">The ID of the user to update.</param>
    /// <param name="updatedUser">The updated user data. Note: If PasswordHash is provided, it should be the plain password, which will be hashed by the server.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated user.</returns>
    /// <response code="200">Returns the updated user.</response>
    /// <response code="404">If the user with the specified ID is not found.</response>
    /// <response code="406">The requested content type is not supported.</response>
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, User updatedUser, CancellationToken cancellationToken)
    {
        var user = await _sender.Send(new UpdateUserCommand(id, updatedUser), cancellationToken);
        return Ok(user);
    }

    // DELETE: api/users/1
    /// <summary>
    /// Deletes a specific user by ID.
    /// </summary>
    /// <param name="id">The ID of the user to delete.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">The user was successfully deleted.</response>
    /// <response code="406">The requested content type is not supported.</response>
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteUserCommand(id), cancellationToken);
        return result ? NoContent() : NotFound();
    }
}
