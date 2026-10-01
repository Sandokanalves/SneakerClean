namespace SneakerClean.Application.DTOs
{
    public record CreateCustomerRequest(
        string Name,
        string Street,
        string Neighborhood,
        string City,
        string ZipCode,
        string Phone
    );

    public record UpdateCustomerRequest(
        string Name,
        string Street,
        string Neighborhood,
        string City,
        string ZipCode,
        string Phone
    );

    public record CustomerDto(
        Guid Id,
        string Name,
        string Street,
        string Neighborhood,
        string City,
        string ZipCode,
        string Phone,
        DateTime CreatedAt,
        bool IsActive
    );

    public record PagedResult<T>(
        IEnumerable<T> Items,
        int PageNumber,
        int PageSize,
        int TotalCount,
        int TotalPages
    );
}
