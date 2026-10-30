using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Superheroes.Dominio;

namespace ApiSuperheroes.ManejoErrores;

public class ManejadorExcepciones : IExceptionHandler
{
    private readonly ILogger<ManejadorExcepciones> _logger;

    public ManejadorExcepciones(ILogger<ManejadorExcepciones> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var problema = exception switch
        {
            ArgumentException ex => new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Solicitud inválida",
                Detail = ex.Message
            },

            HeroeNoExisteException ex => new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Superhéroe no encontrado",
                Detail = ex.Message
            },

            VersionHeroeDesactualizadaException ex => new ProblemDetails
            {
                Status = StatusCodes.Status412PreconditionFailed,
                Title = "Precondición fallida",
                Detail = ex.Message
            },

            _ => new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Error interno del servidor",
                Detail = "Ocurrió un error inesperado al procesar la solicitud."
            }
        };

        if (problema.Status == StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(
                exception,
                "Error inesperado al procesar {Metodo} {Ruta}",
                context.Request.Method,
                context.Request.Path);
        }

        problema.Instance = context.Request.Path.ToString();

        context.Response.StatusCode = problema.Status!.Value;
        context.Response.ContentType = "application/problem+json";

        await context.Response.WriteAsJsonAsync(
            problema,
            cancellationToken);

        return true;
    }
}
