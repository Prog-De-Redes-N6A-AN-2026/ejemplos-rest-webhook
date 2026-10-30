namespace Superheroes.Aplicacion;

public class FiltroHeroes
{
    public string? Nombre { get; set; }
    public string? Creador { get; set; }
    public int? EdadMinima { get; set; }
    public int? EdadMaxima { get; set; }
    public string OrdenarPor { get; set; } = "id";
    public bool Descendente { get; set; }
    public int Pagina { get; set; } = 1;
    public int TamanioPagina { get; set; } = 10;
}
