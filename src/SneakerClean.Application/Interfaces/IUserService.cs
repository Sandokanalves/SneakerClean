using SneakerClean.Application.DTOs;

namespace SneakerClean.Application.Interfaces;

public interface IUserService
{
    Task<UserDto> CreateAsync(CreateUserRequest request);
    Task<UserDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<UserDto>> GetAllAsync();
    Task<AuthResponse?> AuthenticateAsync(LoginRequest request);
}
