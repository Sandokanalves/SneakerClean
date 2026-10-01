using SneakerClean.Application.DTOs;
using SneakerClean.Application.Interfaces;
using SneakerClean.Domain.Entities;
using SneakerClean.Domain.Interfaces;

namespace SneakerClean.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<CustomerDto> CreateAsync(CreateCustomerRequest request)
    {
        var customer = new Customer(
            request.Name,
            request.Street,
            request.Neighborhood,
            request.City,
            request.ZipCode,
            request.Phone
        );

        await _customerRepository.AddAsync(customer);
        return MapToDto(customer);
    }

    public async Task<CustomerDto?> GetByIdAsync(Guid id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        return customer == null ? null : MapToDto(customer);
    }

    public async Task<PagedResult<CustomerDto>> GetPagedAsync(int pageNumber, int pageSize, string? search)
    {
        var (items, totalCount) = await _customerRepository.GetPagedAsync(pageNumber, pageSize, search);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return new PagedResult<CustomerDto>(
            items.Select(MapToDto),
            pageNumber,
            pageSize,
            totalCount,
            totalPages
        );
    }

    public async Task<IEnumerable<CustomerDto>> GetAllAsync()
    {
        var customers = await _customerRepository.GetAllAsync();
        return customers.Select(MapToDto);
    }

    public async Task<CustomerDto?> UpdateAsync(Guid id, UpdateCustomerRequest request)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer == null) return null;

        customer.Update(request.Name, request.Street, request.Neighborhood, request.City, request.ZipCode, request.Phone);
        await _customerRepository.UpdateAsync(customer);
        return MapToDto(customer);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer == null) return false;

        customer.SoftDelete();
        await _customerRepository.UpdateAsync(customer);
        return true;
    }

    private static CustomerDto MapToDto(Customer c) =>
        new(c.Id, c.Name, c.Street, c.Neighborhood, c.City, c.ZipCode, c.Phone, c.CreatedAt, c.IsActive);
}
