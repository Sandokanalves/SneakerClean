using SneakerClean.Application.DTOs;
using SneakerClean.Application.Validators;

namespace SneakerClean.Tests.Unit.Application;

public class RequestValidatorsTests
{
    [Fact]
    public void CreateCustomerRequestValidator_RejectsMissingNameAndPhone()
    {
        var validator = new CreateCustomerRequestValidator();
        var request = new CreateCustomerRequest(" ", "Rua A", "Centro", "Recife", "50000-000", "");

        var result = validator.Validate(request);

        Assert.Contains(result.Errors, error => error.PropertyName == nameof(request.Name));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(request.Phone));
    }

    [Fact]
    public void CreateUserRequestValidator_RejectsInvalidEmailAndRole()
    {
        var validator = new CreateUserRequestValidator();
        var request = new CreateUserRequest("Operador", "email-invalido", "123456", "Gerente");

        var result = validator.Validate(request);

        Assert.Contains(result.Errors, error => error.PropertyName == nameof(request.Email));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(request.Role));
    }

    [Fact]
    public void CreateOrderRequestValidator_RequiresCustomerAndValidItems()
    {
        var validator = new CreateOrderRequestValidator();
        var request = new CreateOrderRequest
        {
            Items =
            [
                new CreateOrderItemDto
                {
                    SneakerBrand = "Nike",
                    SneakerModel = "Air Force 1",
                    Size = 42,
                    ServiceDescription = "Higienização Prime",
                    Price = 0
                }
            ]
        };

        var result = validator.Validate(request);

        Assert.Contains(result.Errors, error => error.PropertyName == nameof(request.CustomerId));
        Assert.Contains(result.Errors, error => error.PropertyName == "Items[0].Price");
    }
}