using System.Collections.Concurrent;
using Superheroes.Aplicacion;
using Superheroes.Dominio;

namespace Superheroes.Infraestructura;

public class RepositorioHeroes : IRepositorioHeroes
{
    private readonly ConcurrentDictionary<int, ModeloHeroe> _heroes = new();
    private int _proximoId;

    public ModeloHeroe Guardar(ModeloHeroe heroe)
    {
        heroe.Id = Interlocked.Increment(ref _proximoId);
        heroe.Version = 1;
        _heroes[heroe.Id] = heroe;
        return heroe;
    }

    public ModeloHeroe? ObtenerPorId(int id) =>
        _heroes.TryGetValue(id, out var heroe) ? heroe : null;

    public List<ModeloHeroe> ObtenerTodos() =>
        _heroes.Values.ToList();

    public ResultadoActualizacionHeroe Actualizar(
        ModeloHeroe heroe,
        int versionEsperada)
    {
        while (true)
        {
            if (!_heroes.TryGetValue(heroe.Id, out var actual))
                return ResultadoActualizacionHeroe.NoExiste;

            if (actual.Version != versionEsperada)
                return ResultadoActualizacionHeroe.VersionIncorrecta;

            var actualizado = new ModeloHeroe
            {
                Id = heroe.Id,
                Version = actual.Version + 1,
                NombreReal = heroe.NombreReal,
                Nombre = heroe.Nombre,
                Edad = heroe.Edad,
                Creadores = heroe.Creadores
            };

            if (_heroes.TryUpdate(
                    heroe.Id,
                    actualizado,
                    actual))
            {
                return ResultadoActualizacionHeroe.Actualizado;
            }
        }
    }

    public bool Eliminar(int id) =>
        _heroes.TryRemove(id, out _);
}
