using FluentValidation;
using SneakerClean.Application.DTOs;
using SneakerClean.Domain.Enums;

namespace SneakerClean.Application.Validators;

public class CreateCustomerRequestValidator : AbstractValidator<CreateCustomerRequest>
{
    public CreateCustomerRequestValidator()
    {
        RuleFor(request => request.Name).NotEmpty().MaximumLength(200).WithMessage("Nome é obrigatório e deve ter até 200 caracteres.");
        RuleFor(request => request.Phone).NotEmpty().MaximumLength(30).WithMessage("Telefone/WhatsApp é obrigatório e deve ter até 30 caracteres.");
        RuleFor(request => request.Street).MaximumLength(300);
        RuleFor(request => request.Neighborhood).MaximumLength(150);
        RuleFor(request => request.City).MaximumLength(150);
        RuleFor(request => request.ZipCode).MaximumLength(10);
    }
}

public class UpdateCustomerRequestValidator : AbstractValidator<UpdateCustomerRequest>
{
    public UpdateCustomerRequestValidator()
    {
        RuleFor(request => request.Name).NotEmpty().MaximumLength(200).WithMessage("Nome é obrigatório e deve ter até 200 caracteres.");
        RuleFor(request => request.Phone).NotEmpty().MaximumLength(30).WithMessage("Telefone/WhatsApp é obrigatório e deve ter até 30 caracteres.");
        RuleFor(request => request.Street).MaximumLength(300);
        RuleFor(request => request.Neighborhood).MaximumLength(150);
        RuleFor(request => request.City).MaximumLength(150);
        RuleFor(request => request.ZipCode).MaximumLength(10);
    }
}

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(request => request.Name).NotEmpty().MaximumLength(150);
        RuleFor(request => request.Email).NotEmpty().EmailAddress().MaximumLength(200);
        RuleFor(request => request.Password).NotEmpty().MinimumLength(6);
        RuleFor(request => request.Role)
            .Must(role => string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase)
                || string.Equals(role, "Operador", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Perfil inválido. Use 'Admin' ou 'Operador'.");
    }
}

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(request => request.Email).NotEmpty().EmailAddress();
        RuleFor(request => request.Password).NotEmpty();
    }
}

public class CreateOrderItemDtoValidator : AbstractValidator<CreateOrderItemDto>
{
    public CreateOrderItemDtoValidator()
    {
        RuleFor(item => item.SneakerBrand).NotEmpty().MaximumLength(100);
        RuleFor(item => item.SneakerModel).NotEmpty().MaximumLength(150);
        RuleFor(item => item.Color).MaximumLength(80);
        RuleFor(item => item.Size).InclusiveBetween(1, 60);
        RuleFor(item => item.ServiceDescription).NotEmpty().MaximumLength(200);
        RuleFor(item => item.Price).GreaterThan(0).LessThanOrEqualTo(99999999.99m);
        RuleFor(item => item.Notes).MaximumLength(500);
    }
}

public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(request => request.CustomerId).NotEmpty().WithMessage("Cliente é obrigatório.");
        RuleFor(request => request.Items).NotEmpty().WithMessage("A OS deve ter pelo menos um item.");
        RuleForEach(request => request.Items).SetValidator(new CreateOrderItemDtoValidator());
    }
}

public class UpdateOrderRequestValidator : AbstractValidator<UpdateOrderRequest>
{
    public UpdateOrderRequestValidator()
    {
        RuleFor(request => request.Status)
            .Must(BeValidStatus)
            .When(request => !string.IsNullOrWhiteSpace(request.Status))
            .WithMessage("Status inválido.");
        RuleForEach(request => request.Items!).SetValidator(new CreateOrderItemDtoValidator())
            .When(request => request.Items != null);
    }

    private static bool BeValidStatus(string? status) =>
        Enum.TryParse<OrderStatus>(status, true, out var parsed) && Enum.IsDefined(parsed);
}

public class UpdateOrderStatusDtoValidator : AbstractValidator<UpdateOrderStatusDto>
{
    public UpdateOrderStatusDtoValidator()
    {
        RuleFor(request => request.Status)
            .Must(status => Enum.TryParse<OrderStatus>(status, true, out var parsed) && Enum.IsDefined(parsed))
            .WithMessage("Status inválido.");
    }
}

public class OrderFilterRequestValidator : AbstractValidator<OrderFilterRequest>
{
    public OrderFilterRequestValidator()
    {
        RuleFor(request => request.Status)
            .Must(BeValidStatus)
            .When(request => !string.IsNullOrWhiteSpace(request.Status))
            .WithMessage("Status inválido.");
        RuleFor(request => request.Month).InclusiveBetween(1, 12).When(request => request.Month.HasValue);
        RuleFor(request => request.Year).GreaterThan(0).When(request => request.Year.HasValue);
    }

    private static bool BeValidStatus(string? status) =>
        Enum.TryParse<OrderStatus>(status, true, out var parsed) && Enum.IsDefined(parsed);
}

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        RuleFor(request => request.Name).NotEmpty().MaximumLength(150);
        RuleFor(request => request.Email).NotEmpty().EmailAddress().MaximumLength(200);
        RuleFor(request => request.Role)
            .Must(role => string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase)
                || string.Equals(role, "Operador", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Perfil inválido. Use 'Admin' ou 'Operador'.");
    }
}