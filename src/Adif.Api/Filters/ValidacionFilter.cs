using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Adif.Api.Filters;

// Corre el validador FluentValidation de cada argumento que tenga uno registrado y responde 400 si falla.
public sealed class ValidacionFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var errores = new Dictionary<string, string[]>();

        foreach (var argumento in context.ActionArguments.Values.Where(a => a is not null))
        {
            if (context.HttpContext.RequestServices.GetService(typeof(IValidator<>).MakeGenericType(argumento!.GetType())) is not IValidator validador) continue;

            var resultado = await validador.ValidateAsync(new ValidationContext<object>(argumento), context.HttpContext.RequestAborted);
            foreach (var grupo in resultado.Errors.GroupBy(e => e.PropertyName)) errores[grupo.Key] = grupo.Select(e => e.ErrorMessage).ToArray();
        }

        if (errores.Count > 0)
        {
            context.Result = new BadRequestObjectResult(new ValidationProblemDetails(errores) { Status = StatusCodes.Status400BadRequest, Title = "La petición tiene datos inválidos." });
            return;
        }

        await next();
    }
}
