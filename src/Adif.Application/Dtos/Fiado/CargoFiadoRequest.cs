// Anotar crédito a un cliente desde WhatsApp.
using System.Text.Json.Serialization;

namespace Adif.Application.Dtos;

public sealed class CargoFiadoRequest : FiadoBaseRequest
{
    [JsonPropertyName("cliente_id")] public int ClienteId { get; set; }
    [JsonPropertyName("nombre")] public string Nombre { get; set; } = string.Empty;
    [JsonPropertyName("monto")] public decimal Monto { get; set; }
    [JsonPropertyName("descripcion")] public string Descripcion { get; set; } = string.Empty;
}
