using System.Text.Json.Serialization;

namespace SneakerClean.Application.DTOs;

public class OrderItemDto
{
    public Guid Id { get; set; }
    public string SneakerBrand { get; set; } = string.Empty;
    public string SneakerModel { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public int Size { get; set; }
    public string ServiceDescription { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? Notes { get; set; }

    [JsonConstructor]
    public OrderItemDto() { }

    public OrderItemDto(Guid id, string sneakerBrand, string sneakerModel, string color, int size, string serviceDescription, decimal price, string? notes)
    {
        Id = id;
        SneakerBrand = sneakerBrand;
        SneakerModel = sneakerModel;
        Color = color;
        Size = size;
        ServiceDescription = serviceDescription;
        Price = price;
        Notes = notes;
    }
}

public class CreateOrderItemDto
{
    public string SneakerBrand { get; set; } = string.Empty;
    public string SneakerModel { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public int Size { get; set; }
    public string ServiceDescription { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? Notes { get; set; }

    [JsonConstructor]
    public CreateOrderItemDto() { }

    public CreateOrderItemDto(string sneakerBrand, string sneakerModel, string color, int size, string serviceDescription, decimal price, string? notes)
    {
        SneakerBrand = sneakerBrand;
        SneakerModel = sneakerModel;
        Color = color;
        Size = size;
        ServiceDescription = serviceDescription;
        Price = price;
        Notes = notes;
    }
}