using Microsoft.EntityFrameworkCore;
using SneakerClean.Domain.Enums;
using SneakerClean.Domain.Interfaces;
using SneakerClean.Infrastructure.Persistence;

namespace SneakerClean.Infrastructure.Repositories;

public class DashboardRepository : IDashboardRepository
{
    private readonly ApplicationDbContext _context;

    public DashboardRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<(decimal TotalRevenueMonth, int OpenOrdersCount, int CompletedOrdersMonthCount)> GetMetricsAsync(int month, int year)
    {
        var ordersThisMonth = await _context.Orders
            .Include(o => o.Items)
            .Where(o => o.CreatedAt.Month == month && o.CreatedAt.Year == year)
            .ToListAsync();

        var totalRevenue = ordersThisMonth
            .Where(o => o.Status == OrderStatus.Completed || o.Status == OrderStatus.Delivered)
            .Sum(o => o.TotalAmount);

        var openCount = await _context.Orders
            .CountAsync(o => o.Status == OrderStatus.Open || o.Status == OrderStatus.InProgress);

        var completedCount = ordersThisMonth
            .Count(o => o.Status == OrderStatus.Completed || o.Status == OrderStatus.Delivered);

        return (totalRevenue, openCount, completedCount);
    }
}
