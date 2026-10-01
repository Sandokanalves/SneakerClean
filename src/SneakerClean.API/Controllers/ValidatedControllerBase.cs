using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace SneakerClean.API.Controllers;

public abstract class ValidatedControllerBase : ControllerBase
{
    protected async Task<IActionResult?> ValidateRequestAsync<TRequest>(TRequest request)
    {
        var validator = HttpContext.RequestServices.GetRequiredService<IValidator<TRequest>>();
        var result = await validator.ValidateAsync(request);

        if (result.IsValid)
            return null;

        return BadRequest(new
        {
            message = "Dados inválidos.",
            errors = result.Errors.Select(error => new
            {
                field = error.PropertyName,
                message = error.ErrorMessage
            })
        });
    }
}