using SneakerClean.Application.DTOs;

namespace SneakerClean.Application.Interfaces;

public interface ICustomerService
{
    Task<CustomerDto> CreateAsync(CreateCustomerRequest request);
    Task<CustomerDto?> GetByIdAsync(Guid id);
    Task<PagedResult<CustomerDto>> GetPagedAsync(int pageNumber, int pageSize, string? search);
    Task<IEnumerable<CustomerDto>> GetAllAsync();
    Task<CustomerDto?> UpdateAsync(Guid id, UpdateCustomerRequest request);
    Task<bool> DeleteAsync(Guid id);
}
