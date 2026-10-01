using Microsoft.EntityFrameworkCore;
using SneakerClean.Domain.Entities;
using SneakerClean.Domain.Interfaces;
using SneakerClean.Infrastructure.Persistence;

namespace SneakerClean.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id) =>
        await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

    public async Task<User?> GetByEmailAsync(string email) =>
        await _context.Users.FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant());

    public async Task<IEnumerable<User>> GetAllAsync() =>
        await _context.Users.OrderBy(u => u.Name).ToListAsync();

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }
}
