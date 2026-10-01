using System.Text.Json.Serialization;

namespace SneakerClean.Application.DTOs;

public class OrderDto
{
    public Guid Id { get; set; }
    public string OrderCode { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public AddressDto? DeliveryAddress { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
    public decimal TotalAmount { get; set; }

    [JsonConstructor]
    public OrderDto() { }
}

public class CreateOrderRequest
{
    public Guid CustomerId { get; set; }
    public AddressDto? DeliveryAddress { get; set; }
    public List<CreateOrderItemDto> Items { get; set; } = new();

    [JsonConstructor]
    public CreateOrderRequest() { }
}

public class UpdateOrderRequest
{
    public string? Status { get; set; }
    public List<CreateOrderItemDto>? Items { get; set; }

    [JsonConstructor]
    public UpdateOrderRequest() { }
}

public class OrderFilterRequest
{
    public string? Status { get; set; }
    public int? Month { get; set; }
    public int? Year { get; set; }
    public Guid? CustomerId { get; set; }
    public string? Search { get; set; }
}

// Keep backward compatibility alias
public class CreateOrderDto : CreateOrderRequest { }
public class UpdateOrderStatusDto
{
    public string Status { get; set; } = string.Empty;
}