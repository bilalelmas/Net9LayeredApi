namespace Net9LayeredApi.Application.DTOs.Products;

public record CreateProductDto(
    Guid UserId,
    string Name,
    string Description,
    decimal Price,
    int Stock
);

public record UpdateProductDto(
    string? Name,
    string? Description,
    decimal? Price,
    int? Stock
);

public record ProductResponseDto(
    Guid Id,
    Guid UserId,
    string Name,
    string Description,
    decimal Price,
    int Stock,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

