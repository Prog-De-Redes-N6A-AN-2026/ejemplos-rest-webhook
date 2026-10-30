using System.Text;
using System.Text.Json;

var webhookUrl = "https://discord.com/api/webhooks/1565112900367028329/n4p2-PeNYyjbBq9197bonhAIjjkj_8omDFXJ7n6NhQaE5qqu4rxlQqSiehWakjMGWMND";

if (string.IsNullOrWhiteSpace(webhookUrl))
{
    Console.WriteLine("Falta configurar la variable webhookUrl.");
    return;
}

using var cliente = new HttpClient();

Console.WriteLine("Escriba mensajes para enviar al webhook de Discord.");
Console.WriteLine("Escriba 'salir' para terminar.");

while (true)
{
    Console.Write("> ");
    var texto = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(texto))
        continue;

    if (texto.Equals("salir", StringComparison.OrdinalIgnoreCase))
        break;

    var mensaje = new { content = texto };
    var json = JsonSerializer.Serialize(mensaje);

    using var contenido = new StringContent(
        json,
        Encoding.UTF8,
        "application/json");

    using var respuesta = await cliente.PostAsync(webhookUrl, contenido);

    Console.WriteLine(
        $"HTTP {(int)respuesta.StatusCode} {respuesta.StatusCode}");

    if (!respuesta.IsSuccessStatusCode)
    {
        var detalle = await respuesta.Content.ReadAsStringAsync();
        Console.WriteLine(detalle);
    }
}
