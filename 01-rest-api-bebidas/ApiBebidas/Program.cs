using System.Collections.Concurrent;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var bebidas = new ConcurrentDictionary<int, Bebida>();
var proximoId = 0;

var api = app.MapGroup("/api/v1/bebidas");

api.MapGet("/", () =>
    Results.Ok(bebidas.Values.OrderBy(bebida => bebida.Id)));

api.MapGet("/{id:int}", (int id) =>
    bebidas.TryGetValue(id, out var bebida)
        ? Results.Ok(bebida)
        : Results.NotFound());

api.MapPost("/", (DatosBebida datos) =>
{
    var errores = Validar(datos);
    if (errores.Count > 0)
        return Results.ValidationProblem(errores);

    var id = Interlocked.Increment(ref proximoId);
    var bebida = new Bebida(
        id,
        datos.Tipo.Trim(),
        datos.Nombre.Trim(),
        datos.Descripcion.Trim());

    bebidas[id] = bebida;

    return Results.Created($"/api/v1/bebidas/{id}", bebida);
});

api.MapPut("/{id:int}", (int id, DatosBebida datos) =>
{
    if (!bebidas.ContainsKey(id))
        return Results.NotFound();

    var errores = Validar(datos);
    if (errores.Count > 0)
        return Results.ValidationProblem(errores);

    bebidas[id] = new Bebida(
        id,
        datos.Tipo.Trim(),
        datos.Nombre.Trim(),
        datos.Descripcion.Trim());

    return Results.NoContent();
});

api.MapDelete("/{id:int}", (int id) =>
    bebidas.TryRemove(id, out _)
        ? Results.NoContent()
        : Results.NotFound());

app.Run();

static Dictionary<string, string[]> Validar(DatosBebida datos)
{
    var errores = new Dictionary<string, string[]>();

    if (string.IsNullOrWhiteSpace(datos.Nombre))
        errores["nombre"] = ["El nombre es obligatorio."];

    if (string.IsNullOrWhiteSpace(datos.Tipo))
        errores["tipo"] = ["El tipo es obligatorio."];

    return errores;
}

record Bebida(int Id, string Tipo, string Nombre, string Descripcion);
record DatosBebida(string Tipo, string Nombre, string Descripcion);
