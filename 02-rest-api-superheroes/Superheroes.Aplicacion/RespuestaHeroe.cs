namespace Superheroes.Aplicacion;

public class RespuestaHeroe
{
    public int Id { get; set; }
    public int Version { get; set; }
    public string NombreReal { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public int Edad { get; set; }
    public string[] Creadores { get; set; } = [];
}
