using System.Text.Json.Serialization;

namespace SneakerClean.Application.DTOs;

public class AddressDto
{
    public string Street { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;

    [JsonConstructor]
    public AddressDto() { }

    public AddressDto(string street, string number, string zipCode, string city, string state)
    {
        Street = street;
        Number = number;
        ZipCode = zipCode;
        City = city;
        State = state;
    }
}