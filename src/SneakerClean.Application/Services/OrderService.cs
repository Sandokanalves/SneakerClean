using SneakerClean.Application.DTOs;
using SneakerClean.Application.Interfaces;
using SneakerClean.Domain.Entities;
using SneakerClean.Domain.Enums;
using SneakerClean.Domain.Interfaces;
using SneakerClean.Domain.ValueObjects;

namespace SneakerClean.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICustomerRepository _customerRepository;

    public OrderService(IOrderRepository orderRepository, ICustomerRepository customerRepository)
    {
        _orderRepository = orderRepository;
        _customerRepository = customerRepository;
    }

    public async Task<OrderDto?> GetByIdAsync(Guid id)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        return order == null ? null : MapToDto(order);
    }

    public async Task<IEnumerable<OrderDto>> GetAllAsync()
    {
        var orders = await _orderRepository.GetAllAsync();
        return orders.Select(MapToDto);
    }

    public async Task<IEnumerable<OrderDto>> GetFilteredAsync(OrderFilterRequest filter)
    {
        OrderStatus? status = null;
        if (!string.IsNullOrWhiteSpace(filter.Status) && Enum.TryParse<OrderStatus>(filter.Status, true, out var s))
            status = s;

        var orders = await _orderRepository.GetFilteredAsync(
            status,
            filter.Month,
            filter.Year,
            filter.CustomerId,
            filter.Search
        );

        return orders.Select(MapToDto);
    }

    public async Task<OrderDto> CreateAsync(CreateOrderRequest request)
    {
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId)
            ?? throw new KeyNotFoundException($"Cliente com Id '{request.CustomerId}' não encontrado.");

        if (!customer.IsActive)
            throw new InvalidOperationException("Não é possível criar uma OS para um cliente inativo.");

        if (request.Items == null || !request.Items.Any())
            throw new ArgumentException("A ordem de serviço deve ter pelo menos um item.");

        Address? address = request.DeliveryAddress != null
            ? new Address(request.DeliveryAddress.Street, request.DeliveryAddress.Number ?? "", request.DeliveryAddress.ZipCode, request.DeliveryAddress.City, request.DeliveryAddress.State ?? "")
            : null;

        var order = new Order(request.CustomerId, address);

        foreach (var item in request.Items)
            order.AddItem(item.SneakerBrand, item.SneakerModel, item.Color, item.Size, item.ServiceDescription, item.Price, item.Notes);

        await _orderRepository.AddAsync(order);

        // Load customer navigation for mapping
        var created = await _orderRepository.GetByIdAsync(order.Id);
        return MapToDto(created!);
    }

    public async Task<OrderDto?> UpdateAsync(Guid id, UpdateOrderRequest request)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        if (order == null) return null;

        if (!string.IsNullOrWhiteSpace(request.Status) && Enum.TryParse<OrderStatus>(request.Status, true, out var newStatus))
            order.UpdateStatus(newStatus);

        if (request.Items != null && request.Items.Any())
        {
            order.ClearItems();
            foreach (var item in request.Items)
                order.AddItem(item.SneakerBrand, item.SneakerModel, item.Color, item.Size, item.ServiceDescription, item.Price, item.Notes);
        }

        await _orderRepository.UpdateAsync(order);
        return MapToDto(order);
    }

    public async Task<bool> UpdateStatusAsync(Guid id, string statusStr)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        if (order == null) return false;

        if (!Enum.TryParse<OrderStatus>(statusStr, true, out var newStatus))
            return false;

        order.UpdateStatus(newStatus);
        await _orderRepository.UpdateAsync(order);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        if (order == null) return false;

        order.UpdateStatus(OrderStatus.Cancelled);
        await _orderRepository.UpdateAsync(order);
        return true;
    }

    private static OrderDto MapToDto(Order order)
    {
        AddressDto? addressDto = order.DeliveryAddress != null
            ? new AddressDto(order.DeliveryAddress.Street, order.DeliveryAddress.Number, order.DeliveryAddress.ZipCode, order.DeliveryAddress.City, order.DeliveryAddress.State)
            : null;

        var itemsDto = order.Items.Select(i => new OrderItemDto(
            i.Id, i.SneakerBrand, i.SneakerModel, i.Color, i.Size, i.ServiceDescription, i.Price, i.Notes
        )).ToList();

        return new OrderDto
        {
            Id = order.Id,
            OrderCode = order.OrderCode,
            CustomerId = order.CustomerId,
            CustomerName = order.Customer?.Name ?? string.Empty,
            CustomerPhone = order.Customer?.Phone ?? string.Empty,
            DeliveryAddress = addressDto,
            Status = order.Status.ToString(),
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,
            Items = itemsDto,
            TotalAmount = order.TotalAmount
        };
    }
}