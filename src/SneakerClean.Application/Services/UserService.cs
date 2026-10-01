using BCrypt.Net;
using SneakerClean.Application.DTOs;
using SneakerClean.Application.Interfaces;
using SneakerClean.Domain.Entities;
using SneakerClean.Domain.Enums;
using SneakerClean.Domain.Interfaces;

namespace SneakerClean.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;

    public UserService(IUserRepository userRepository, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    public async Task<UserDto> CreateAsync(CreateUserRequest request)
    {
        var existing = await _userRepository.GetByEmailAsync(request.Email);
        if (existing != null)
            throw new InvalidOperationException($"Já existe um usuário com o e-mail '{request.Email}'.");

        if (!Enum.TryParse<UserRole>(request.Role, true, out var role))
            throw new ArgumentException($"Perfil inválido: '{request.Role}'. Use 'Admin' ou 'Operador'.");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var user = new User(request.Name, request.Email, passwordHash, role);

        await _userRepository.AddAsync(user);
        return MapToDto(user);
    }

    public async Task<UserDto?> GetByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user == null ? null : MapToDto(user);
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(MapToDto);
    }

    public async Task<AuthResponse?> AuthenticateAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null || !user.IsActive)
            return null;

        bool valid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if (!valid)
            return null;

        var token = _tokenService.GenerateToken(user.Id.ToString(), user.Email, user.Role.ToString());
        return new AuthResponse(token, MapToDto(user));
    }

    private static UserDto MapToDto(User user) =>
        new(user.Id, user.Name, user.Email, user.Role.ToString(), user.IsActive, user.CreatedAt);
}
