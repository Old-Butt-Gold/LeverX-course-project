using EER.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EER.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private static readonly Dictionary<Guid, User> _users = new();
    private readonly IPasswordHasher<User> _passwordHasher;

    public UsersController(IPasswordHasher<User> passwordHasher)
    {
        _passwordHasher = passwordHasher;
    }

    // GET: api/users
    /// <summary>
    /// Retrieves all users.
    /// </summary>
    /// <returns>A list of all users.</returns>
    /// <response code="200">Returns the list of users.</response>
    [ProducesResponseType(typeof(IEnumerable<User>), StatusCodes.Status200OK)]
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_users.Values);
    }

    // GET: api/users/1
    /// <summary>
    /// Retrieves a specific user by ID.
    /// </summary>
    /// <param name="id">The ID of the user to retrieve.</param>
    /// <returns>The requested user if found.</returns>
    /// <response code="200">Returns the requested user.</response>
    /// <response code="404">If the user with the specified ID is not found.</response>
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        return _users.TryGetValue(id, out var user) 
            ? Ok(user) 
            : NotFound();
    }

    // POST: api/users
    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="user">The user to create. Note: PasswordHash should contain the plain password, which will be hashed by the server.</param>
    /// <returns>The created user.</returns>
    /// <response code="201">Returns the created user ID.</response>
    [ProducesResponseType(typeof(User), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost]
    public IActionResult Create(User user)
    {
        user.Id = Guid.NewGuid(); 
        user.CreatedAt = DateTime.UtcNow;
        user.PasswordHash = _passwordHasher.HashPassword(user, user.PasswordHash);
        _users[user.Id] = user;
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    // PUT: api/users/1
    /// <summary>
    /// Updates an existing user by ID.
    /// </summary>
    /// <param name="id">The ID of the user to update.</param>
    /// <param name="updatedUser">The updated user data. Note: If PasswordHash is provided, it should be the plain password, which will be hashed by the server.</param>
    /// <returns>The updated user.</returns>
    /// <response code="200">Returns the updated user.</response>
    /// <response code="404">If the user with the specified ID is not found.</response>
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPut("{id:guid}")]
    public IActionResult Update(Guid id, User updatedUser)
    {
        if (!_users.TryGetValue(id, out var user))
        {
            return NotFound();
        }
       
        user.Email = updatedUser.Email;
        user.FullName = updatedUser.FullName;
        user.PasswordHash = _passwordHasher.HashPassword(updatedUser, updatedUser.PasswordHash);
        return Ok(user);
    }

    // DELETE: api/users/1
    /// <summary>
    /// Deletes a specific user by ID.
    /// </summary>
    /// <param name="id">The ID of the user to delete.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">The user was successfully deleted.</response>
    /// <response code="404">If the user with the specified ID is not found.</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        return !_users.Remove(id) 
            ? NotFound() 
            : NoContent();
    }
}