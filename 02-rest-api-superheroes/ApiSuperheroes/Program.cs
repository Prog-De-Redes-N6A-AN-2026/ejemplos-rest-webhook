using ApiSuperheroes.Autenticacion;
using ApiSuperheroes.ManejoErrores;
using Superheroes.Aplicacion;
using Superheroes.Infraestructura;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ManejadorExcepciones>();

builder.Services.AddSingleton<IRepositorioHeroes, RepositorioHeroes>();
builder.Services.AddScoped<IServicioHeroes, ServicioHeroes>();
builder.Services.AddScoped<IAdaptadorHeroes, AdaptadorHeroes>();

var token =
    Environment.GetEnvironmentVariable("API_TOKEN")
    ?? "pdr-2026";

builder.Services.AddSingleton(
    new ConfiguracionToken(token));

var app = builder.Build();

app.UseExceptionHandler();
app.UseMiddleware<AutenticacionTokenMiddleware>();

app.MapControllers();

app.Run();
