namespace Superheroes.Dominio;

public class HeroeNoExisteException : Exception
{
    public HeroeNoExisteException()
        : base("El superhéroe no existe.")
    {
    }
}
