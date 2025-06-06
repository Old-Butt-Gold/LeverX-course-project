using System.Net.Mime;
using EER.Application.Abstractions.Security;
using EER.Application.Dto.Security.Login;
using EER.Application.Dto.Security.RefreshToken;
using EER.Application.Dto.Security.RegisterAdmin;
using EER.Application.Dto.Security.RegisterUser;
using EER.Application.Extensions;
using EER.Application.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace EER.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "AnyRole")]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<AuthenticationController> _logger;

    public AuthenticationController(IAuthenticationService authenticationService, IOptions<JwtSettings> options,
        ILogger<AuthenticationController> logger)
    {
        _jwtSettings = options.Value;
        _authenticationService = authenticationService;
        _logger = logger;
    }

    // POST: api/authentication/register
    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// /// <param name="user">
    /// The user to create.
    /// Note: <c>PasswordHash</c> should contain the plain password, which will be hashed by the server.
    /// </param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>HTTP 201 Created if successful.</returns>
    /// <response code="201">User was successfully created.</response>
    /// <response code="400">Invalid user data.</response>
    /// <response code="406">The requested content type is not supported.</response>
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterUserDto user, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Registration user with email: {Email} and role: {Role}", user.Email, user.UserRole.ToString());
        await _authenticationService.RegisterUserAsync(user, cancellationToken);
        return Created();
    }

    // POST: api/authentication/register-admin
    /// <summary>
    /// Creates a new administrator account.
    /// </summary>
    /// <param name="adminDto">
    /// The administrator data to register.
    /// Note: <c>PasswordHash</c> should contain the plain password, which will be hashed by the server.
    /// Note: Only current available administrators can use it
    /// </param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>HTTP 201 Created if successful.</returns>
    /// <response code="201">Administrator was successfully created.</response>
    /// <response code="400">Invalid administrator data.</response>
    /// <response code="401">Unauthorized: requires Admin role.</response>
    /// <response code="406">The requested content type is not supported.</response>
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpPost("register-admin")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> RegisterAdmin(RegisterAdminDto adminDto, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Registration admin with email: {Email}", adminDto.Email);
        await _authenticationService.RegisterAdminAsync(adminDto, cancellationToken);

        return Created();
    }

    // POST: api/authentication/login
    /// <summary>
    /// Authenticates a user and returns an access token.
    /// </summary>
    /// <param name="loginDto">
    /// The credentials for login.
    /// </param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The access token and sets a refresh token cookie.</returns>
    /// <response code="200">Returns the access token.</response>
    /// <response code="400">Invalid login request.</response>
    /// <response code="401">Unauthorized: invalid credentials.</response>
    /// <response code="406">The requested content type is not supported.</response>
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginUserDto loginDto, CancellationToken cancellationToken)
    {
        var result = await _authenticationService.LoginAsync(loginDto, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Login failed for email: {Email}", loginDto.Email);
            return Unauthorized();
        }

        _logger.LogInformation("User logged in. Email: {Email}, UserId: {UserId}", loginDto.Email, result.UserId);

        Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            Expires = DateTime.UtcNow.AddSeconds(_jwtSettings.RefreshExpirySeconds),
            SameSite = SameSiteMode.Strict,
        });

        return Ok(new { result.AccessToken });
    }

    // POST: api/authentication/refresh-token
    /// <summary>
    /// Refreshes the access token using the refresh token.
    /// </summary>
    /// <param name="accessToken">The expired access token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>New access token and updates the refresh token cookie.</returns>
    /// <response code="200">Returns the new access token.</response>
    /// <response code="400">Invalid refresh request.</response>
    /// <response code="401">Unauthorized: refresh token not found or invalid.</response>
    /// <response code="406">The requested content type is not supported.</response>
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken([FromBody] string accessToken, CancellationToken cancellationToken)
    {
        if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
        {
            _logger.LogWarning("Logout failed: Refresh token not found");
            return BadRequest("Refresh token not found");
        }

        var dto = new RefreshTokenDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
        };

        var result = await _authenticationService.RefreshTokenAsync(dto, cancellationToken);

        Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            Expires = DateTime.UtcNow.AddSeconds(_jwtSettings.RefreshExpirySeconds),
            SameSite = SameSiteMode.Strict
        });

        _logger.LogInformation("User logged out. UserId: {UserId}", User.GetUserId());

        return Ok(new { result.AccessToken });
    }

    // POST: api/authentication/logout
    /// <summary>
    /// Logs out the current user by revoking their refresh token.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Refresh token revoked and logged out.</response>
    /// <response code="400">Refresh token not found in cookies.</response>
    /// <response code="401">Unauthorized: user not authenticated.</response>
    /// <response code="406">The requested content type is not supported.</response>
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
            return BadRequest("Refresh token not found");

        await _authenticationService.LogoutAsync(refreshToken, cancellationToken);

        Response.Cookies.Delete("refreshToken", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict
        });

        return NoContent();
    }

    // POST: api/authentication/logout-all
    /// <summary>
    /// Logs out the current user from all devices by revoking all their refresh tokens.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">All refresh tokens revoked for the user.</response>
    /// <response code="401">Unauthorized: user not authenticated.</response>
    /// <response code="406">The requested content type is not supported.</response>
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [HttpPost("logout-all")]
    public async Task<IActionResult> LogoutAll(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        await _authenticationService.LogoutAllAsync(userId, cancellationToken);

        Response.Cookies.Delete("refreshToken", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict
        });

        return NoContent();
    }
}
