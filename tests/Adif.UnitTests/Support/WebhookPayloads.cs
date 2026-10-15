using System.Text.Json;

namespace Adif.UnitTests.Support;

// Payloads de ejemplo con la forma que envía Kapso (sin datos reales).
public static class WebhookPayloads
{
    public const string PhoneNumberId = "100200300400";
    public const string Operador = "50400000001";

    public static string Texto(string texto, string id = "wamid.1", string direccion = "inbound") => JsonSerializer.Serialize(new
    {
        @event = "whatsapp.message.received",
        data = new
        {
            phone_number_id = PhoneNumberId,
            message = new { id, from = Operador, type = "text", text = new { body = texto }, kapso = new { direction = direccion } }
        }
    });

    public static string Boton(string botonId) => JsonSerializer.Serialize(new
    {
        phone_number_id = PhoneNumberId,
        message = new { id = "wamid.b", from = Operador, interactive = new { button_reply = new { id = botonId, title = "x" } } }
    });

    public static string Lista(string filaId) => JsonSerializer.Serialize(new
    {
        conversation = new { phone_number_id = PhoneNumberId },
        message = new { id = "wamid.l", from = Operador, interactive = new { list_reply = new { id = filaId } } }
    });
}
