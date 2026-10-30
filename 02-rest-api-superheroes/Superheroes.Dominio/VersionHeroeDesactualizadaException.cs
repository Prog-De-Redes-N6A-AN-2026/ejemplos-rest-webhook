namespace Superheroes.Dominio;

public class VersionHeroeDesactualizadaException : Exception
{
    public VersionHeroeDesactualizadaException()
        : base("El recurso cambió desde que fue leído. Vuelva a obtenerlo antes de actualizar.")
    {
    }
}
