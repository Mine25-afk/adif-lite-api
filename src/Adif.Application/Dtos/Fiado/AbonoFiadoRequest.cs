// Registrar un abono de un cliente desde WhatsApp.
using System.Text.Json.Serialization;

namespace Adif.Application.Dtos;

public sealed class AbonoFiadoRequest : FiadoBaseRequest
{
    [JsonPropertyName("cliente_id")] public int ClienteId { get; set; }
    [JsonPropertyName("nombre")] public string Nombre { get; set; } = string.Empty;
    [JsonPropertyName("monto")] public decimal Monto { get; set; }
}
