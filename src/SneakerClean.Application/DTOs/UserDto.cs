namespace SneakerClean.Application.DTOs
{
    public record CreateUserRequest(
        string Name,
        string Email,
        string Password,
        string Role
    );

    public record UpdateUserRequest(
        string Name,
        string Email,
        string Role
    );

    public record UserDto(
        Guid Id,
        string Name,
        string Email,
        string Role,
        bool IsActive,
        DateTime CreatedAt
    );

    public record LoginRequest(
        string Email,
        string Password
    );

    public record AuthResponse(
        string Token,
        UserDto User
    );
}
