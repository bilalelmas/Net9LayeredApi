namespace Net9LayeredApi.Application.DTOs.Reviews;

public record CreateReviewDto(
    Guid UserId,
    Guid ProductId,
    int Rating,
    string Comment
);

public record UpdateReviewDto(
    int? Rating,
    string? Comment
);

public record ReviewResponseDto(
    Guid Id,
    Guid UserId,
    Guid ProductId,
    int Rating,
    string Comment,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

