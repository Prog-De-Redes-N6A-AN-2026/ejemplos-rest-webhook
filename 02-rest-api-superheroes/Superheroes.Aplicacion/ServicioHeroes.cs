using Superheroes.Dominio;

namespace Superheroes.Aplicacion;

public class ServicioHeroes : IServicioHeroes
{
    private readonly IRepositorioHeroes _repositorio;

    public ServicioHeroes(IRepositorioHeroes repositorio)
    {
        _repositorio = repositorio;
    }

    public ModeloHeroe Guardar(ModeloHeroe heroe) =>
        _repositorio.Guardar(heroe);

    public ModeloHeroe? ObtenerPorId(int id) =>
        _repositorio.ObtenerPorId(id);

    public ResultadoPaginado<ModeloHeroe> ObtenerTodos(FiltroHeroes filtro)
    {
        ValidarFiltro(filtro);

        IEnumerable<ModeloHeroe> consulta = _repositorio.ObtenerTodos();

        if (!string.IsNullOrWhiteSpace(filtro.Nombre))
        {
            consulta = consulta.Where(heroe =>
                heroe.Nombre.Contains(
                    filtro.Nombre,
                    StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(filtro.Creador))
        {
            consulta = consulta.Where(heroe =>
                heroe.Creadores.Any(creador =>
                    creador.Contains(
                        filtro.Creador,
                        StringComparison.OrdinalIgnoreCase)));
        }

        if (filtro.EdadMinima.HasValue)
            consulta = consulta.Where(heroe =>
                heroe.Edad >= filtro.EdadMinima.Value);

        if (filtro.EdadMaxima.HasValue)
            consulta = consulta.Where(heroe =>
                heroe.Edad <= filtro.EdadMaxima.Value);

        consulta = AplicarOrden(consulta, filtro);

        var total = consulta.Count();

        var elementos = consulta
            .Skip((filtro.Pagina - 1) * filtro.TamanioPagina)
            .Take(filtro.TamanioPagina)
            .ToList();

        return new ResultadoPaginado<ModeloHeroe>(
            elementos,
            total,
            filtro.Pagina,
            filtro.TamanioPagina);
    }

    public ResultadoActualizacionHeroe Actualizar(
        ModeloHeroe heroe,
        int versionEsperada) =>
        _repositorio.Actualizar(heroe, versionEsperada);

    public bool Eliminar(int id) =>
        _repositorio.Eliminar(id);

    private static IEnumerable<ModeloHeroe> AplicarOrden(
        IEnumerable<ModeloHeroe> consulta,
        FiltroHeroes filtro)
    {
        return filtro.OrdenarPor.ToLowerInvariant() switch
        {
            "nombre" => filtro.Descendente
                ? consulta.OrderByDescending(heroe => heroe.Nombre)
                : consulta.OrderBy(heroe => heroe.Nombre),

            "edad" => filtro.Descendente
                ? consulta.OrderByDescending(heroe => heroe.Edad)
                : consulta.OrderBy(heroe => heroe.Edad),

            "id" => filtro.Descendente
                ? consulta.OrderByDescending(heroe => heroe.Id)
                : consulta.OrderBy(heroe => heroe.Id),

            _ => throw new ArgumentException(
                "ordenarPor debe ser 'id', 'nombre' o 'edad'.")
        };
    }

    private static void ValidarFiltro(FiltroHeroes filtro)
    {
        if (filtro.Pagina < 1)
            throw new ArgumentException("La página debe ser mayor o igual a 1.");

        if (filtro.TamanioPagina is < 1 or > 50)
            throw new ArgumentException(
                "El tamaño de página debe estar entre 1 y 50.");

        if (filtro.EdadMinima < 0 || filtro.EdadMaxima < 0)
            throw new ArgumentException("Las edades no pueden ser negativas.");

        if (filtro.EdadMinima.HasValue &&
            filtro.EdadMaxima.HasValue &&
            filtro.EdadMinima > filtro.EdadMaxima)
        {
            throw new ArgumentException(
                "La edad mínima no puede ser mayor que la edad máxima.");
        }
    }
}
