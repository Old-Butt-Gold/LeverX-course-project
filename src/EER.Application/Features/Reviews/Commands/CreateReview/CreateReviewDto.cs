namespace EER.Application.Features.Reviews.Commands.CreateReview;

public record CreateReviewDto
{
    public byte Rating { get; init; } // 1-5
    public string? Comment { get; init; }
}
