namespace SneakerClean.Domain.Events;

public record OrderCreatedEvent(Guid OrderId, string CustomerName, decimal TotalAmount, DateTime CreatedAt);