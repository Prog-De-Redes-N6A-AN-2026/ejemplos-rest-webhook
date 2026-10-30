namespace Superheroes.Aplicacion;

public record ResultadoPaginado<T>(
    IReadOnlyList<T> Elementos,
    int Total,
    int Pagina,
    int TamanioPagina);
