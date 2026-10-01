using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SneakerClean.Application.DTOs;
using SneakerClean.Application.Interfaces;
using SneakerClean.Domain.Events;
using SneakerClean.Infrastructure.Messaging;

namespace SneakerClean.API.Controllers;

[ApiController]
[Route("api/orders")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly RabbitMqPublisher _rabbitMqPublisher;

    public OrdersController(IOrderService orderService, RabbitMqPublisher rabbitMqPublisher)
    {
        _orderService = orderService;
        _rabbitMqPublisher = rabbitMqPublisher;
    }

    /// <summary>
    /// Lista ordens de serviço com filtros opcionais.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? status = null,
        [FromQuery] int? month = null,
        [FromQuery] int? year = null,
        [FromQuery] Guid? customerId = null,
        [FromQuery] string? search = null)
    {
        var filter = new OrderFilterRequest
        {
            Status = status,
            Month = month,
            Year = year,
            CustomerId = customerId,
            Search = search
        };

        var orders = await _orderService.GetFilteredAsync(filter);
        return Ok(orders);
    }

    /// <summary>
    /// Obtém uma ordem de serviço por ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var order = await _orderService.GetByIdAsync(id);
        if (order == null) return NotFound(new { message = "Ordem de serviço não encontrada." });
        return Ok(order);
    }

    /// <summary>
    /// Cria uma nova OS com cliente selecionado e lista de tênis/serviços.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequest request)
    {
        if (request.CustomerId == Guid.Empty)
            return BadRequest(new { message = "Cliente é obrigatório." });
        if (request.Items == null || !request.Items.Any())
            return BadRequest(new { message = "A OS deve ter pelo menos um item." });

        try
        {
            var createdOrder = await _orderService.CreateAsync(request);

            var orderEvent = new OrderCreatedEvent(
                createdOrder.Id,
                createdOrder.CustomerName,
                createdOrder.TotalAmount,
                DateTime.UtcNow
            );
            await _rabbitMqPublisher.PublishOrderCreatedAsync(orderEvent);

            return CreatedAtAction(nameof(GetById), new { id = createdOrder.Id }, createdOrder);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Atualiza status e/ou itens de uma OS.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateOrderRequest request)
    {
        try
        {
            var updated = await _orderService.UpdateAsync(id, request);
            if (updated == null) return NotFound(new { message = "Ordem de serviço não encontrada." });
            return Ok(updated);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Atualiza apenas o status de uma OS (PATCH).
    /// </summary>
    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateOrderStatusDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Status))
            return BadRequest(new { message = "Status é obrigatório." });

        try
        {
            var updated = await _orderService.UpdateStatusAsync(id, dto.Status);
            if (!updated) return NotFound(new { message = "Ordem não encontrada ou status inválido." });
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Cancela (soft delete) uma OS.
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _orderService.DeleteAsync(id);
        if (!deleted) return NotFound(new { message = "Ordem de serviço não encontrada." });
        return NoContent();
    }
}