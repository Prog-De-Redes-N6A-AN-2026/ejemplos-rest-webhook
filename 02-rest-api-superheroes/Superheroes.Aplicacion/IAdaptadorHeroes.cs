namespace Superheroes.Aplicacion;

public interface IAdaptadorHeroes
{
    RespuestaHeroe Crear(SolicitudHeroe solicitud);
    RespuestaHeroe Obtener(int id);
    ResultadoPaginado<RespuestaHeroe> ObtenerTodos(FiltroHeroes filtro);
    RespuestaHeroe Actualizar(
        int id,
        SolicitudHeroe solicitud,
        int versionEsperada);
    void Eliminar(int id);
}
