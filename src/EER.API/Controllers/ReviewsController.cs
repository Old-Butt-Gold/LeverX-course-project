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
    /// <summary>
    /// Retrieves all reviews.
    /// </summary>
    /// <returns>A list of all reviews.</returns>
    /// <response code="200">Returns the list of reviews.</response>
    [ProducesResponseType(typeof(IEnumerable<Review>), StatusCodes.Status200OK)]
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_reviews.Values);
    }

    // GET: api/reviews/1
    /// <summary>
    /// Retrieves a specific review by ID.
    /// </summary>
    /// <param name="id">The ID of the review to retrieve.</param>
    /// <returns>The requested review if found.</returns>
    /// <response code="200">Returns the requested review.</response>
    /// <response code="404">If the review with the specified ID is not found.</response>
    [ProducesResponseType(typeof(Review), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{id:long}")]
    public IActionResult GetById(long id)
    {
        return _reviews.TryGetValue(id, out var review) 
            ? Ok(review) 
            : NotFound();
    }

    // POST: api/reviews
    /// <summary>
    /// Creates a new review.
    /// </summary>
    /// <param name="review">The review to create.</param>
    /// <returns>The created review.</returns>
    /// <response code="201">Returns the created review ID.</response>
    [ProducesResponseType(typeof(Review), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
    /// <summary>
    /// Updates an existing review by ID.
    /// </summary>
    /// <param name="id">The ID of the review to update.</param>
    /// <param name="updatedReview">The updated review data.</param>
    /// <returns>The updated review.</returns>
    /// <response code="200">Returns the updated review.</response>
    /// <response code="404">If the review with the specified ID is not found.</response>
    [ProducesResponseType(typeof(Review), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPut("{id:long}")]
    public IActionResult Update(long id, Review updatedReview)
    {
        if (!_reviews.TryGetValue(id, out var review))
        {
            return NotFound();
        }
        
        review.Rating = updatedReview.Rating;
        review.Comment = updatedReview.Comment;
        review.UpdatedAt = DateTime.UtcNow;
        return Ok(review);
    }

    // DELETE: api/reviews/1
    /// <summary>
    /// Deletes a specific review by ID.
    /// </summary>
    /// <param name="id">The ID of the review to delete.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">The review was successfully deleted.</response>
    /// <response code="404">If the review with the specified ID is not found.</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpDelete("{id:long}")]
    public IActionResult Delete(long id)
    {
        return !_reviews.Remove(id) 
            ? NotFound() 
            : NoContent();
    }
}