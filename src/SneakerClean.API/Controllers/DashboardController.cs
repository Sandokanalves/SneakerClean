using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SneakerClean.Application.Interfaces;

namespace SneakerClean.API.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    /// <summary>
    /// Retorna métricas do dashboard (faturamento, OS abertas, OS concluídas no mês).
    /// </summary>
    [HttpGet("metrics")]
    public async Task<IActionResult> GetMetrics(
        [FromQuery] int? month = null,
        [FromQuery] int? year = null)
    {
        var metrics = await _dashboardService.GetMetricsAsync(month, year);
        return Ok(metrics);
    }
}
