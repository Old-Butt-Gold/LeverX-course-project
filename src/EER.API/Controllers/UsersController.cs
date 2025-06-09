using System.Net.Mime;
using EER.Application.Extensions;
using EER.Application.Features.Users.Commands.DeleteUser;
using EER.Application.Features.Users.Commands.UpdateUser;
using EER.Application.Features.Users.Queries.GetAllUsers;
using EER.Application.Features.Users.Queries.GetUserById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace EER.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "AnyRole")]
public sealed class UsersController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ILogger<UsersController> _logger;

    public UsersController(ISender sender, ILogger<UsersController> logger)
    {
        _sender = sender;
        _logger = logger;
    }

    // GET: api/users
    /// <summary>
    /// Retrieves all users.
    /// </summary>
    /// <returns>A list of all users.</returns>
    /// <response code="200">Returns the list of users.</response>
    /// <response code="406">The requested content type is not supported.</response>
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        _logger.LogInformation("User {UserId} requested all users", User.GetUserId());
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
    [ProducesResponseType(typeof(UserDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("User {UserId} requested user ID: {TargetUserId}", User.GetUserId(), id);
        var user = await _sender.Send(new GetUserByIdQuery(id), cancellationToken);
        return user is not null ? Ok(user) : NotFound();
    }

    // PUT: api/users/1
    /// <summary>
    /// Updates an existing user by ID.
    /// </summary>
    /// <param name="updatedUser">The updated user data. Note: If PasswordHash is provided, it should be the plain password, which will be hashed by the server.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated user.</returns>
    /// <response code="200">Returns the updated user.</response>
    /// <response code="404">If the user with the specified ID is not found.</response>
    /// <response code="406">The requested content type is not supported.</response>
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [ProducesResponseType(typeof(UserUpdatedDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpPut]
    public async Task<IActionResult> Update(UpdateUserDto updatedUser, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        if (userId != updatedUser.Id)
            return Unauthorized();

        _logger.LogInformation("User {UserId} updating his information", userId);

        var user = await _sender.Send(new UpdateUserCommand(updatedUser), cancellationToken);

        _logger.LogInformation("User {UserId} updated user ID: {TargetUserId}", userId, updatedUser.Id);
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
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        _logger.LogInformation("User {UserId} deleting user ID: {TargetUserId}", userId, id);

        var result = await _sender.Send(new DeleteUserCommand(id), cancellationToken);
        return result ? NoContent() : NotFound();
    }

    // GET: api/users/me
    /// <summary>
    /// Gets information about user from JWT token
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User profile information</returns>
    /// <response code="200">Returns the requested user information.</response>
    /// <response code="404">If the user with the specified ID is not found.</response>
    /// <response code="406">The requested content type is not supported.</response>
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [ProducesResponseType(typeof(UserDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpGet("me")]
    public async Task<IActionResult> Me(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        _logger.LogInformation("User {UserId} gets his profile information", userId);

        var user = await _sender.Send(new GetUserByIdQuery(userId), cancellationToken);
        return user is not null ? Ok(user) : NotFound();
    }
}
