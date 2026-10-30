using Microsoft.AspNetCore.Mvc;
using Superheroes.Aplicacion;

namespace ApiSuperheroes.Controllers;

[ApiController]
[Route("api/v1/heroes")]
public class HeroesController : ControllerBase
{
    private readonly IAdaptadorHeroes _adaptador;

    public HeroesController(IAdaptadorHeroes adaptador)
    {
        _adaptador = adaptador;
    }

    [HttpGet]
    public ActionResult<ResultadoPaginado<RespuestaHeroe>> ObtenerTodos(
        [FromQuery] FiltroHeroes filtro)
    {
        return Ok(_adaptador.ObtenerTodos(filtro));
    }

    [HttpGet("{id:int}", Name = "ObtenerHeroe")]
    public ActionResult<RespuestaHeroe> ObtenerPorId(int id)
    {
        var respuesta = _adaptador.Obtener(id);
        var etag = CrearETag(respuesta.Version);

        Response.Headers["ETag"] = etag;
        Response.Headers["Cache-Control"] = "private, max-age=30";

        var ifNoneMatch =
            Request.Headers["If-None-Match"].ToString();

        if (ifNoneMatch == etag)
            return StatusCode(
                StatusCodes.Status304NotModified);

        return Ok(respuesta);
    }

    [HttpPost]
    public ActionResult<RespuestaHeroe> Crear(
        SolicitudHeroe solicitud)
    {
        var respuesta = _adaptador.Crear(solicitud);

        Response.Headers["ETag"] =
            CrearETag(respuesta.Version);

        return CreatedAtRoute(
            "ObtenerHeroe",
            new { id = respuesta.Id },
            respuesta);
    }

    [HttpPut("{id:int}")]
    public IActionResult Actualizar(
        int id,
        SolicitudHeroe solicitud)
    {
        var ifMatch =
            Request.Headers["If-Match"].ToString();

        var versionEsperada = LeerVersionETag(ifMatch);

        var actualizado = _adaptador.Actualizar(
            id,
            solicitud,
            versionEsperada);

        Response.Headers["ETag"] =
            CrearETag(actualizado.Version);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public IActionResult Eliminar(int id)
    {
        _adaptador.Eliminar(id);
        return NoContent();
    }

    private static string CrearETag(int version) =>
        $"\"v{version}\"";

    private static int LeerVersionETag(string etag)
    {
        if (string.IsNullOrWhiteSpace(etag))
        {
            throw new ArgumentException(
                "Debe enviar un ETag válido en el header If-Match, por ejemplo: \"v1\".");
        }

        var valor = etag.Trim();

        if (valor.Length < 4 ||
            valor[0] != '"' ||
            valor[^1] != '"' ||
            valor[1] != 'v' ||
            !int.TryParse(valor[2..^1], out var version))
        {
            throw new ArgumentException(
                "Debe enviar un ETag válido en el header If-Match, por ejemplo: \"v1\".");
        }

        return version;
    }
}
