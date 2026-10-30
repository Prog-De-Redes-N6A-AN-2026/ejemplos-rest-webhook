using Superheroes.Dominio;

namespace Superheroes.Aplicacion;

public interface IRepositorioHeroes
{
    ModeloHeroe Guardar(ModeloHeroe heroe);
    ModeloHeroe? ObtenerPorId(int id);
    List<ModeloHeroe> ObtenerTodos();
    ResultadoActualizacionHeroe Actualizar(
        ModeloHeroe heroe,
        int versionEsperada);
    bool Eliminar(int id);
}
