// Estado de la conversación de un operador con el bot de fiado.
namespace Adif.Application.Dtos;

public sealed class SesionBot
{
    public string Paso { get; set; } = string.Empty;
    public string Accion { get; set; } = string.Empty;
    public int ClienteId { get; set; }
    public string ClienteNombre { get; set; } = string.Empty;
    public string NombrePendiente { get; set; } = string.Empty;
    public DateTime Tocada { get; set; } = DateTime.Now;
}
