namespace SneakerClean.Application.DTOs
{
    public record DashboardMetricsDto(
        decimal TotalRevenueMonth,
        int OpenOrdersCount,
        int CompletedOrdersMonthCount,
        int Month,
        int Year
    );
}
