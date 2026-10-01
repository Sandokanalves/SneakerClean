using SneakerClean.Application.DTOs;
using SneakerClean.Application.Interfaces;
using SneakerClean.Domain.Interfaces;

namespace SneakerClean.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IDashboardRepository _dashboardRepository;

    public DashboardService(IDashboardRepository dashboardRepository)
    {
        _dashboardRepository = dashboardRepository;
    }

    public async Task<DashboardMetricsDto> GetMetricsAsync(int? month, int? year)
    {
        var now = DateTime.UtcNow;
        var m = month ?? now.Month;
        var y = year ?? now.Year;

        var (totalRevenue, openCount, completedCount) = await _dashboardRepository.GetMetricsAsync(m, y);

        return new DashboardMetricsDto(totalRevenue, openCount, completedCount, m, y);
    }
}
