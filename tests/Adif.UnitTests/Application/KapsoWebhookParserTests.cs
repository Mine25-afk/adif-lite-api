using Adif.Application.Services;
using Adif.UnitTests.Support;

namespace Adif.UnitTests.Application;

public class KapsoWebhookParserTests
{
    [Fact]
    public void Lee_un_texto_envuelto_en_event_y_data()
    {
        var (mensaje, _) = KapsoWebhookParser.Parsear(WebhookPayloads.Texto("hola"), null);

        mensaje.Should().NotBeNull();
        mensaje!.PhoneNumberId.Should().Be(WebhookPayloads.PhoneNumberId);
        mensaje.From.Should().Be(WebhookPayloads.Operador);
        mensaje.Texto.Should().Be("hola");
        mensaje.Saliente.Should().BeFalse();
    }

    [Fact]
    public void Lee_la_respuesta_de_un_boton() => KapsoWebhookParser.Parsear(WebhookPayloads.Boton("menu_fiado"), null).Mensaje!.RespuestaId.Should().Be("menu_fiado");

    [Fact]
    public void Lee_la_respuesta_de_una_lista_con_phone_id_en_conversation()
    {
        var mensaje = KapsoWebhookParser.Parsear(WebhookPayloads.Lista("cli_7"), null).Mensaje!;

        mensaje.RespuestaId.Should().Be("cli_7");
        mensaje.PhoneNumberId.Should().Be(WebhookPayloads.PhoneNumberId);
    }

    [Fact]
    public void Marca_los_mensajes_salientes() => KapsoWebhookParser.Parsear(WebhookPayloads.Texto("x", direccion: "outbound"), null).Mensaje!.Saliente.Should().BeTrue();

    [Theory]
    [InlineData("no es json")]
    [InlineData("")]
    [InlineData("[1,2]")]
    [InlineData("{\"message\":{}}")]
    public void Cuerpos_invalidos_se_ignoran(string cuerpo) => KapsoWebhookParser.Parsear(cuerpo, null).Mensaje.Should().BeNull();

    [Fact]
    public void Otros_eventos_se_ignoran()
    {
        var (mensaje, motivo) = KapsoWebhookParser.Parsear(WebhookPayloads.Boton("x"), "whatsapp.message.delivered");

        mensaje.Should().BeNull();
        motivo.Should().Contain("delivered");
    }
}
