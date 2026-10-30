using Microsoft.AspNetCore.Mvc;

namespace ApiSuperheroes.Autenticacion;

public class AutenticacionTokenMiddleware
{
    private readonly RequestDelegate _siguiente;

    public AutenticacionTokenMiddleware(RequestDelegate siguiente)
    {
        _siguiente = siguiente;
    }

    public async Task InvokeAsync(
        HttpContext context,
        ConfiguracionToken configuracion)
    {
        if (!RequiereToken(context.Request))
        {
            await _siguiente(context);
            return;
        }

        var authorization =
            context.Request.Headers["Authorization"].ToString();

        const string prefijo = "Bearer ";

        var tokenValido =
            authorization.StartsWith(
                prefijo,
                StringComparison.OrdinalIgnoreCase) &&
            authorization[prefijo.Length..].Trim() ==
                configuracion.Valor;

        if (!tokenValido)
        {
            context.Response.StatusCode =
                StatusCodes.Status401Unauthorized;
            context.Response.ContentType =
                "application/problem+json";

            await context.Response.WriteAsJsonAsync(
                new ProblemDetails
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Title = "No autenticado",
                    Detail =
                        "Falta un Bearer token válido en el header Authorization."
                });

            return;
        }

        await _siguiente(context);
    }

    private static bool RequiereToken(HttpRequest request)
    {
        if (!request.Path.StartsWithSegments("/api/v1/heroes"))
            return false;

        return HttpMethods.IsPost(request.Method) ||
               HttpMethods.IsPut(request.Method) ||
               HttpMethods.IsDelete(request.Method);
    }
}
