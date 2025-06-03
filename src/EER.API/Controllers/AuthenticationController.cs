using System.Net.Mime;
using EER.Application.Features.Authentication.Commands.LoginUser;
using EER.Application.Features.Users.Commands.CreateUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EER.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
public class AuthenticationController : ControllerBase
{
    private readonly ISender _sender;

    public AuthenticationController(ISender sender)
    {
        _sender = sender;
    }

    // POST: api/authentication/register
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
    [ProducesResponseType(typeof(UserCreatedDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpPost("register")]
    public async Task<IActionResult> Register(CreateUserDto user, CancellationToken cancellationToken)
    {
        var command = new CreateUserCommand(user);

        await _sender.Send(command, cancellationToken);

        return Created();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginUserDto loginDto, CancellationToken cancellationToken)
    {
        var command = new LoginUserCommand(loginDto);

        var result = await _sender.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return Unauthorized();

        return Ok(result);
    }
}
