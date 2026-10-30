using Superheroes.Dominio;

namespace Superheroes.Aplicacion;

public interface IServicioHeroes
{
    ModeloHeroe Guardar(ModeloHeroe heroe);
    ModeloHeroe? ObtenerPorId(int id);
    ResultadoPaginado<ModeloHeroe> ObtenerTodos(FiltroHeroes filtro);
    ResultadoActualizacionHeroe Actualizar(
        ModeloHeroe heroe,
        int versionEsperada);
    bool Eliminar(int id);
}
