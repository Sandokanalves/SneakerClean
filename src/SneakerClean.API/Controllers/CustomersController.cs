using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SneakerClean.Application.DTOs;
using SneakerClean.Application.Interfaces;

namespace SneakerClean.API.Controllers;

[ApiController]
[Route("api/customers")]
[Authorize]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    /// <summary>
    /// Cadastra um novo cliente.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCustomerRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest(new { message = "Nome é obrigatório." });
        if (string.IsNullOrWhiteSpace(request.Phone))
            return BadRequest(new { message = "Telefone/WhatsApp é obrigatório." });

        try
        {
            var customer = await _customerService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Lista clientes com paginação e filtro por nome ou telefone.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 20;

        var result = await _customerService.GetPagedAsync(page, pageSize, search);
        return Ok(result);
    }

    /// <summary>
    /// Lista todos os clientes ativos (sem paginação, para dropdowns).
    /// </summary>
    [HttpGet("all")]
    public async Task<IActionResult> GetAllActive()
    {
        var customers = await _customerService.GetAllAsync();
        return Ok(customers);
    }

    /// <summary>
    /// Obtém um cliente por ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var customer = await _customerService.GetByIdAsync(id);
        if (customer == null) return NotFound(new { message = "Cliente não encontrado." });
        return Ok(customer);
    }

    /// <summary>
    /// Atualiza os dados de um cliente.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCustomerRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest(new { message = "Nome é obrigatório." });
        if (string.IsNullOrWhiteSpace(request.Phone))
            return BadRequest(new { message = "Telefone/WhatsApp é obrigatório." });

        var customer = await _customerService.UpdateAsync(id, request);
        if (customer == null) return NotFound(new { message = "Cliente não encontrado." });
        return Ok(customer);
    }

    /// <summary>
    /// Desativa (soft delete) um cliente.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _customerService.DeleteAsync(id);
        if (!deleted) return NotFound(new { message = "Cliente não encontrado." });
        return NoContent();
    }
}
