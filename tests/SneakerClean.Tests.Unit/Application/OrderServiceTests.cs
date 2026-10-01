using FluentAssertions;
using Moq;
using SneakerClean.Application.DTOs;
using SneakerClean.Application.Services;
using SneakerClean.Domain.Entities;
using SneakerClean.Domain.Interfaces;
using Xunit;

namespace SneakerClean.Tests.Unit.Application;

public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _orderRepoMock;
    private readonly Mock<ICustomerRepository> _customerRepoMock;
    private readonly OrderService _orderService;

    public OrderServiceTests()
    {
        _orderRepoMock = new Mock<IOrderRepository>();
        _customerRepoMock = new Mock<ICustomerRepository>();
        _orderService = new OrderService(_orderRepoMock.Object, _customerRepoMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnValidOrderDto_WhenDataIsValid()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var customer = new Customer("Sandokan Oliveira", "Rua A", "Centro", "Recife", "50000-000", "81999999999");

        _customerRepoMock
            .Setup(x => x.GetByIdAsync(customerId))
            .ReturnsAsync(customer);

        _orderRepoMock
            .Setup(x => x.AddAsync(It.IsAny<Order>()))
            .Returns(Task.CompletedTask);

        // Simulate GetByIdAsync returning the created order
        _orderRepoMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Guid id) =>
            {
                var order = new Order(customerId, null);
                // Use reflection to set Customer navigation
                typeof(Order).GetProperty("Customer")!.SetValue(order, customer);
                order.AddItem("Nike", "Air Jordan 1", "Preto/Vermelho", 42, "Higienização Prime", 120.00m);
                return order;
            });

        var request = new CreateOrderRequest
        {
            CustomerId = customerId,
            Items = new List<CreateOrderItemDto>
            {
                new() { SneakerBrand = "Nike", SneakerModel = "Air Jordan 1", Color = "Preto/Vermelho", Size = 42, ServiceDescription = "Higienização Prime", Price = 120.00m }
            }
        };

        // Act
        var result = await _orderService.CreateAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.CustomerId.Should().Be(customerId);
        result.TotalAmount.Should().Be(120.00m);
        _orderRepoMock.Verify(x => x.AddAsync(It.IsAny<Order>()), Times.Once);
    }
}