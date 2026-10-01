using SneakerClean.Domain.Entities;

namespace SneakerClean.Domain.Interfaces
{
    public interface ICustomerRepository
    {
        Task<Customer?> GetByIdAsync(Guid id);
        Task<(IEnumerable<Customer> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, string? search);
        Task<IEnumerable<Customer>> GetAllAsync();
        Task AddAsync(Customer customer);
        Task UpdateAsync(Customer customer);
    }
}
