using EER.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace EER.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReviewsController : ControllerBase
{
    private static readonly Dictionary<long, Review> _reviews = new();
    private static long _idCounter;

    // GET: api/reviews
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_reviews.Values);
    }

    // GET: api/reviews/1
    [HttpGet("{id:long}")]
    public IActionResult GetById(long id)
    {
        return _reviews.TryGetValue(id, out var review) 
            ? Ok(review) 
            : NotFound();
    }

    // POST: api/reviews
    [HttpPost]
    public IActionResult Create(Review review)
    {
        review.Id = Interlocked.Increment(ref _idCounter);
        review.UpdatedAt = DateTime.UtcNow;
        review.CreatedAt = DateTime.UtcNow;
        _reviews[review.Id] = review;
        return CreatedAtAction(nameof(GetById), new { id = review.Id }, review);
    }

    // PUT: api/reviews/1
    [HttpPut("{id:long}")]
    public IActionResult Update(long id, Review updatedReview)
    {
        if (!_reviews.TryGetValue(id, out var review))
        {
            return NoContent();
        }
        
        review.Rating = updatedReview.Rating;
        review.Comment = updatedReview.Comment;
        review.UpdatedAt = DateTime.UtcNow;
        return Ok(review);
    }

    // DELETE: api/reviews/1
    [HttpDelete("{id:long}")]
    public IActionResult Delete(long id)
    {
        return !_reviews.Remove(id) 
            ? NotFound() 
            : NoContent();
    }
}