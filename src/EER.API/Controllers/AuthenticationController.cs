using System.Net.Mime;
using EER.Application.Extensions;
using EER.Application.Features.Authentication.Commands.LoginUser;
using EER.Application.Features.Authentication.Commands.Logout;
using EER.Application.Features.Authentication.Commands.LogoutAll;
using EER.Application.Features.Authentication.Commands.RefreshToken;
using EER.Application.Features.Users.Commands.CreateUser;
using EER.Application.Settings;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace EER.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AuthenticationController : ControllerBase
{
    private readonly ISender _sender;
    private readonly JwtSettings _jwtSettings;

    public AuthenticationController(ISender sender, IOptions<JwtSettings> options)
    {
        _sender = sender;
        _jwtSettings = options.Value;
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
    [AllowAnonymous]
    public async Task<IActionResult> Register(CreateUserDto user, CancellationToken cancellationToken)
    {
        var command = new CreateUserCommand(user);

        await _sender.Send(command, cancellationToken);

        return Created();
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginUserDto loginDto, CancellationToken cancellationToken)
    {
        var command = new LoginUserCommand(loginDto);

        var result = await _sender.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return Unauthorized();

        Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            Expires = DateTime.UtcNow.AddSeconds(_jwtSettings.RefreshExpirySeconds),
            SameSite = SameSiteMode.Strict,
        });

        return Ok(new { result.AccessToken });
    }

    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken([FromBody] string accessToken, CancellationToken cancellationToken)
    {
        if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
            return Unauthorized("Refresh token not found");

        var dto = new RefreshTokenDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
        };

        var command = new RefreshTokenCommand(dto);
        var result = await _sender.Send(command, cancellationToken);

        Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            Expires = DateTime.UtcNow.AddSeconds(_jwtSettings.RefreshExpirySeconds),
            SameSite = SameSiteMode.Strict
        });

        return Ok(new { result.AccessToken });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
            return BadRequest("Refresh token not found");

        await _sender.Send(new LogoutCommand(refreshToken), cancellationToken);

        Response.Cookies.Delete("refreshToken", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict
        });

        return NoContent();
    }

    [HttpPost("logout-all")]
    public async Task<IActionResult> LogoutAll(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        await _sender.Send(new LogoutAllCommand(userId), cancellationToken);

        Response.Cookies.Delete("refreshToken", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict
        });

        return NoContent();
    }
}
