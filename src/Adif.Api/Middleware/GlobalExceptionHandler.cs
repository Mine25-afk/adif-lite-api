using Adif.Application.Common.Exceptions;
using Adif.Domain.Common;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Adif.Api.Middleware;

// Traduce cada excepción a ProblemDetails (RFC 9457); el detalle técnico solo va al log.
public sealed class GlobalExceptionHandler(IProblemDetailsService problemDetails, ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (status, titulo) = exception switch
        {
            ValidationException => (StatusCodes.Status400BadRequest, "La petición tiene datos inválidos."),
            BadHttpRequestException => (StatusCodes.Status400BadRequest, "Petición mal formada."),
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "No autorizado."),
            ForbiddenException => (StatusCodes.Status403Forbidden, "Acceso denegado."),
            NotFoundException => (StatusCodes.Status404NotFound, "No encontrado."),
            ConflictException => (StatusCodes.Status409Conflict, "Conflicto."),
            DbUpdateConcurrencyException => (StatusCodes.Status409Conflict, "El registro cambió mientras se guardaba; vuelve a intentarlo."),
            DomainException => (StatusCodes.Status422UnprocessableEntity, "Regla de negocio incumplida."),
            _ => (StatusCodes.Status500InternalServerError, "Ocurrió un error inesperado.")
        };

        if (status >= 500) logger.LogError(exception, "Error no controlado en {Metodo} {Ruta}", httpContext.Request.Method, httpContext.Request.Path);
        else logger.LogInformation("Petición rechazada con {Status}: {Mensaje}", status, exception.Message);

        var problema = exception is ValidationException validacion
            ? new ValidationProblemDetails(validacion.Errors.GroupBy(e => e.PropertyName).ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray()))
            : new ProblemDetails();
        problema.Status = status;
        problema.Title = titulo;
        problema.Detail = status >= 500 || exception is DbUpdateConcurrencyException ? null : exception.Message;

        httpContext.Response.StatusCode = status;
        return await problemDetails.TryWriteAsync(new ProblemDetailsContext { HttpContext = httpContext, ProblemDetails = problema, Exception = exception });
    }
}
