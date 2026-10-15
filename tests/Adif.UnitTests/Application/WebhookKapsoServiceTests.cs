using Adif.Application.Dtos;
using Adif.Application.Services;
using Adif.Domain.Entities;
using Adif.Infrastructure.WhatsApp;
using Adif.UnitTests.Support;
using Microsoft.Extensions.Logging.Abstractions;

namespace Adif.UnitTests.Application;

public class WebhookKapsoServiceTests
{
    private const string Secreto = "secreto-del-negocio-1";
    private readonly IWhatsappContextService _contexto = Substitute.For<IWhatsappContextService>();
    private readonly IFiadoBotService _bot = Substitute.For<IFiadoBotService>();
    private readonly WebhookKapsoService _servicio;

    public WebhookKapsoServiceTests()
    {
        _contexto.ObtenerNumeroAsync(WebhookPayloads.PhoneNumberId, Arg.Any<CancellationToken>()).Returns(new NegocioWhatsapp { NegocioId = 3, PhoneNumberId = WebhookPayloads.PhoneNumberId, WebhookSecret = Secreto });
        _bot.ProcesarAsync(Arg.Any<NegocioDelMensaje>(), Arg.Any<MensajeEntrante>(), Arg.Any<CancellationToken>()).Returns([new EnvioWhatsapp { Simulado = true }]);
        _servicio = new WebhookKapsoService(_contexto, _bot, new MemoriaDeduplicadorMensajes(), NullLogger<WebhookKapsoService>.Instance);
    }

    private Task<WebhookResultado> Enviar(string cuerpo, string? firma = null) => _servicio.ProcesarAsync(cuerpo, firma ?? FirmaWebhook.Calcular(cuerpo, Secreto), null);

    [Fact]
    public async Task Firma_valida_entrega_el_mensaje_al_bot_del_negocio()
    {
        var resultado = await Enviar(WebhookPayloads.Texto("hola"));

        resultado.Autorizado.Should().BeTrue();
        resultado.Procesado.Should().BeTrue();
        await _bot.Received(1).ProcesarAsync(Arg.Is<NegocioDelMensaje>(n => n.NegocioId == 3), Arg.Is<MensajeEntrante>(m => m.Texto == "hola"), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Firma_invalida_no_autoriza_ni_llama_al_bot()
    {
        var resultado = await Enviar(WebhookPayloads.Texto("hola"), firma: "deadbeef");

        resultado.Autorizado.Should().BeFalse();
        await _bot.DidNotReceiveWithAnyArgs().ProcesarAsync(default!, default!, default);
    }

    [Fact]
    public async Task Numero_no_registrado_no_se_puede_verificar_y_se_rechaza()
    {
        _contexto.ObtenerNumeroAsync(WebhookPayloads.PhoneNumberId, Arg.Any<CancellationToken>()).Returns((NegocioWhatsapp?)null);

        (await Enviar(WebhookPayloads.Texto("hola"))).Autorizado.Should().BeFalse();
    }

    [Fact]
    public async Task Reintento_del_mismo_mensaje_se_procesa_una_sola_vez()
    {
        var cuerpo = WebhookPayloads.Texto("hola", id: "wamid.repetido");

        await Enviar(cuerpo);
        var segundo = await Enviar(cuerpo);

        segundo.Procesado.Should().BeFalse();
        segundo.Motivo.Should().Contain("duplicado");
        await _bot.ReceivedWithAnyArgs(1).ProcesarAsync(default!, default!, default);
    }

    [Fact]
    public async Task Mensajes_salientes_se_ignoran() => (await Enviar(WebhookPayloads.Texto("eco", direccion: "outbound"))).Procesado.Should().BeFalse();

    [Fact]
    public async Task Error_del_bot_no_se_propaga_para_evitar_reintentos()
    {
        _bot.ProcesarAsync(Arg.Any<NegocioDelMensaje>(), Arg.Any<MensajeEntrante>(), Arg.Any<CancellationToken>()).Returns<IReadOnlyList<EnvioWhatsapp>>(_ => throw new InvalidOperationException("falla"));

        var resultado = await Enviar(WebhookPayloads.Texto("hola", id: "wamid.error"));

        resultado.Autorizado.Should().BeTrue();
        resultado.Procesado.Should().BeFalse();
    }
}
