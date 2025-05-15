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
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_users.Values);
    }

    // GET: api/users/1
    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        return _users.TryGetValue(id, out var user) 
            ? Ok(user) 
            : NotFound();;
    }

    // POST: api/users
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
    [HttpPut("{id:guid}")]
    public IActionResult Update(Guid id, User updatedUser)
    {
        if (!_users.TryGetValue(id, out var user))
        {
            return NoContent();
        }
       
        user.Email = updatedUser.Email;
        user.FullName = updatedUser.FullName;
        user.PasswordHash = _passwordHasher.HashPassword(updatedUser, updatedUser.PasswordHash);
        return Ok(user);
    }

    // DELETE: api/users/1
    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        return !_users.Remove(id) 
            ? NotFound() 
            : NoContent();
    }
}