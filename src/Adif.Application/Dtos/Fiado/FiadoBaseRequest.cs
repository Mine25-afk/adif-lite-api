// Campos que las herramientas de Kapso envían en toda operación de fiado.
using System.Text.Json.Serialization;

namespace Adif.Application.Dtos;

public abstract class FiadoBaseRequest
{
    [JsonPropertyName("phone_number_id")] public string PhoneNumberId { get; set; } = string.Empty;
    [JsonPropertyName("from")] public string From { get; set; } = string.Empty;
}
