using Microsoft.EntityFrameworkCore;
using SneakerClean.Domain.Entities;
using SneakerClean.Domain.Interfaces;
using SneakerClean.Infrastructure.Persistence;

namespace SneakerClean.Infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly ApplicationDbContext _context;

    public CustomerRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Customer?> GetByIdAsync(Guid id) =>
        await _context.Customers.FirstOrDefaultAsync(c => c.Id == id);

    public async Task<(IEnumerable<Customer> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, string? search)
    {
        var query = _context.Customers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var lower = search.ToLower();
            query = query.Where(c =>
                c.Name.ToLower().Contains(lower) ||
                c.Phone.Contains(lower));
        }

        var total = await query.CountAsync();
        var items = await query
            .OrderBy(c => c.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<IEnumerable<Customer>> GetAllAsync() =>
        await _context.Customers.Where(c => c.IsActive).OrderBy(c => c.Name).ToListAsync();

    public async Task AddAsync(Customer customer)
    {
        await _context.Customers.AddAsync(customer);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Customer customer)
    {
        _context.Customers.Update(customer);
        await _context.SaveChangesAsync();
    }
}
