namespace Superheroes.Aplicacion;

public class SolicitudHeroe
{
    public string NombreReal { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public int Edad { get; set; }
    public string[] Creadores { get; set; } = [];
}
