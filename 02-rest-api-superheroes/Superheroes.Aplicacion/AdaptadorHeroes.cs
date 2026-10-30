using Superheroes.Dominio;

namespace Superheroes.Aplicacion;

public class AdaptadorHeroes : IAdaptadorHeroes
{
    private readonly IServicioHeroes _servicio;

    public AdaptadorHeroes(IServicioHeroes servicio)
    {
        _servicio = servicio;
    }

    public RespuestaHeroe Crear(SolicitudHeroe solicitud)
    {
        ValidarSolicitud(solicitud);
        return ARespuesta(_servicio.Guardar(AModelo(solicitud)));
    }

    public RespuestaHeroe Obtener(int id)
    {
        ValidarId(id);

        var heroe = _servicio.ObtenerPorId(id)
            ?? throw new HeroeNoExisteException();

        return ARespuesta(heroe);
    }

    public ResultadoPaginado<RespuestaHeroe> ObtenerTodos(FiltroHeroes filtro)
    {
        var resultado = _servicio.ObtenerTodos(filtro);

        return new ResultadoPaginado<RespuestaHeroe>(
            resultado.Elementos.Select(ARespuesta).ToList(),
            resultado.Total,
            resultado.Pagina,
            resultado.TamanioPagina);
    }

    public RespuestaHeroe Actualizar(
        int id,
        SolicitudHeroe solicitud,
        int versionEsperada)
    {
        ValidarId(id);
        ValidarSolicitud(solicitud);

        var heroe = AModelo(solicitud);
        heroe.Id = id;

        var resultado = _servicio.Actualizar(
            heroe,
            versionEsperada);

        if (resultado == ResultadoActualizacionHeroe.NoExiste)
            throw new HeroeNoExisteException();

        if (resultado == ResultadoActualizacionHeroe.VersionIncorrecta)
            throw new VersionHeroeDesactualizadaException();

        return Obtener(id);
    }

    public void Eliminar(int id)
    {
        ValidarId(id);

        if (!_servicio.Eliminar(id))
            throw new HeroeNoExisteException();
    }

    private static void ValidarId(int id)
    {
        if (id <= 0)
            throw new ArgumentException("El id debe ser mayor que cero.");
    }

    private static void ValidarSolicitud(SolicitudHeroe solicitud)
    {
        if (string.IsNullOrWhiteSpace(solicitud.Nombre))
            throw new ArgumentException("El nombre del superhéroe es obligatorio.");

        if (solicitud.Edad < 0)
            throw new ArgumentException("La edad no puede ser negativa.");
    }

    private static ModeloHeroe AModelo(SolicitudHeroe solicitud) => new()
    {
        NombreReal = solicitud.NombreReal,
        Nombre = solicitud.Nombre,
        Edad = solicitud.Edad,
        Creadores = solicitud.Creadores
    };

    private static RespuestaHeroe ARespuesta(ModeloHeroe heroe) => new()
    {
        Id = heroe.Id,
        Version = heroe.Version,
        NombreReal = heroe.NombreReal,
        Nombre = heroe.Nombre,
        Edad = heroe.Edad,
        Creadores = heroe.Creadores
    };
}
