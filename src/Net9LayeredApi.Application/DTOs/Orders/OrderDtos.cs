namespace Net9LayeredApi.Application.DTOs.Orders;

public record CreateOrderItemDto(
    Guid ProductId,
    int Quantity
);

public record CreateOrderDto(
    Guid UserId,
    List<CreateOrderItemDto> Items
);

public record UpdateOrderDto(
    string? Status
);

public record OrderItemResponseDto(
    Guid Id,
    Guid ProductId,
    int Quantity,
    decimal UnitPrice,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record OrderResponseDto(
    Guid Id,
    Guid UserId,
    decimal TotalPrice,
    string Status,
    List<OrderItemResponseDto> Items,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

