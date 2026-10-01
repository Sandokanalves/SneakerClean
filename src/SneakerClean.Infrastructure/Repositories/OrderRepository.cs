using Microsoft.EntityFrameworkCore;
using SneakerClean.Domain.Entities;
using SneakerClean.Domain.Enums;
using SneakerClean.Domain.Interfaces;
using SneakerClean.Infrastructure.Persistence;

namespace SneakerClean.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly ApplicationDbContext _context;

    public OrderRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Order?> GetByIdAsync(Guid id) =>
        await _context.Orders
            .Include(o => o.Items)
            .Include(o => o.Customer)
            .FirstOrDefaultAsync(o => o.Id == id);

    public async Task<IEnumerable<Order>> GetAllAsync() =>
        await _context.Orders
            .Include(o => o.Items)
            .Include(o => o.Customer)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

    public async Task<IEnumerable<Order>> GetFilteredAsync(OrderStatus? status, int? month, int? year, Guid? customerId, string? search)
    {
        var query = _context.Orders
            .Include(o => o.Items)
            .Include(o => o.Customer)
            .AsQueryable();

        if (status.HasValue)
            query = query.Where(o => o.Status == status.Value);

        if (month.HasValue && year.HasValue)
            query = query.Where(o => o.CreatedAt.Month == month.Value && o.CreatedAt.Year == year.Value);
        else if (month.HasValue)
            query = query.Where(o => o.CreatedAt.Month == month.Value);
        else if (year.HasValue)
            query = query.Where(o => o.CreatedAt.Year == year.Value);

        if (customerId.HasValue)
            query = query.Where(o => o.CustomerId == customerId.Value);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var lower = search.ToLower();
            query = query.Where(o =>
                o.OrderCode.ToLower().Contains(lower) ||
                o.Customer.Name.ToLower().Contains(lower) ||
                o.Customer.Phone.Contains(lower));
        }

        return await query.OrderByDescending(o => o.CreatedAt).ToListAsync();
    }

    public async Task AddAsync(Order order)
    {
        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Order order)
    {
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var order = await GetByIdAsync(id);
        if (order != null)
        {
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
        }
    }
}