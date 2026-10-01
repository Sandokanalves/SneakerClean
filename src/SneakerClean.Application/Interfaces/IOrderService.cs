using SneakerClean.Application.DTOs;

namespace SneakerClean.Application.Interfaces;

public interface IOrderService
{
    Task<OrderDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<OrderDto>> GetAllAsync();
    Task<IEnumerable<OrderDto>> GetFilteredAsync(OrderFilterRequest filter);
    Task<OrderDto> CreateAsync(CreateOrderRequest request);
    Task<OrderDto?> UpdateAsync(Guid id, UpdateOrderRequest request);
    Task<bool> UpdateStatusAsync(Guid id, string status);
    Task<bool> DeleteAsync(Guid id);
}