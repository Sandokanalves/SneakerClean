namespace SneakerClean.Domain.Interfaces
{
    public interface IDashboardRepository
    {
        Task<(decimal TotalRevenueMonth, int OpenOrdersCount, int CompletedOrdersMonthCount)> GetMetricsAsync(int month, int year);
    }
}
