namespace Net9LayeredApi.Application.DTOs.Users;

public record CreateUserDto(string Username, string Email, string Password, string Role);

public record UpdateUserDto(string? Username, string? Email, string? Password, string? Role);

public record UserResponseDto(
    Guid Id,
    string Username,
    string Email,
    string Role,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

