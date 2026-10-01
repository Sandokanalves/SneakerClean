using SneakerClean.Application.DTOs;

namespace SneakerClean.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardMetricsDto> GetMetricsAsync(int? month, int? year);
}
