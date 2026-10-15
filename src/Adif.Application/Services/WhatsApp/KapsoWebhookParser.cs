using System.Text.Json;
using System.Text.Json.Nodes;
using Adif.Application.Dtos;

namespace Adif.Application.Services;

// Extrae el mensaje entrante del payload de Kapso (directo o envuelto en { event, data }).
public static class KapsoWebhookParser
{
    public const string EventoMensajeRecibido = "whatsapp.message.received";

    public static (MensajeEntrante? Mensaje, string Motivo) Parsear(string cuerpo, string? eventoHeader)
    {
        JsonNode? raiz;
        try { raiz = string.IsNullOrWhiteSpace(cuerpo) ? null : JsonNode.Parse(cuerpo); }
        catch (JsonException) { return (null, "El cuerpo no es JSON."); }

        if (raiz is not JsonObject) return (null, "Sin cuerpo.");

        var evento = Texto(raiz["event"]);
        if (evento.Length == 0) evento = eventoHeader ?? string.Empty;
        if (evento.Length > 0 && evento != EventoMensajeRecibido) return (null, $"Evento ignorado: {evento}.");

        var datos = raiz["data"] ?? raiz;
        var mensaje = datos["message"];
        var phoneNumberId = Texto(datos["phone_number_id"]);
        if (phoneNumberId.Length == 0) phoneNumberId = Texto(datos["conversation"]?["phone_number_id"]);

        if (mensaje is null || phoneNumberId.Length == 0) return (null, "Payload sin mensaje o phone_number_id.");

        var respuestaId = Texto(mensaje["interactive"]?["button_reply"]?["id"]);
        if (respuestaId.Length == 0) respuestaId = Texto(mensaje["interactive"]?["list_reply"]?["id"]);

        return (new MensajeEntrante
        {
            Id = Texto(mensaje["id"]),
            PhoneNumberId = phoneNumberId,
            From = Texto(mensaje["from"]),
            Texto = Texto(mensaje["text"]?["body"]),
            RespuestaId = respuestaId,
            Saliente = Texto(mensaje["kapso"]?["direction"]) == "outbound"
        }, string.Empty);
    }

    private static string Texto(JsonNode? nodo) => nodo is JsonValue v && v.TryGetValue<string>(out var s) ? s : string.Empty;
}
