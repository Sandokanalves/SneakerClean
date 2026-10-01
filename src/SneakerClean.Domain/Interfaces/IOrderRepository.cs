using SneakerClean.Domain.Entities;
using SneakerClean.Domain.Enums;

namespace SneakerClean.Domain.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(Guid id);
        Task<IEnumerable<Order>> GetFilteredAsync(OrderStatus? status, int? month, int? year, Guid? customerId, string? search);
        Task<IEnumerable<Order>> GetAllAsync();
        Task AddAsync(Order order);
        Task UpdateAsync(Order order);
        Task DeleteAsync(Guid id);
    }
}