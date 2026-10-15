// Alta de cliente desde WhatsApp.
using System.Text.Json.Serialization;

namespace Adif.Application.Dtos;

public sealed class CrearClienteFiadoRequest : FiadoBaseRequest
{
    [JsonPropertyName("nombre")] public string Nombre { get; set; } = string.Empty;
    [JsonPropertyName("telefono")] public string Telefono { get; set; } = string.Empty;
}
