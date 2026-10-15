using Adif.Application.Dtos;
using Adif.Application.Services;
using Adif.Domain.Common;
using Adif.Infrastructure.WhatsApp;
using Adif.UnitTests.Support;

namespace Adif.UnitTests.Application;

public class FiadoBotServiceTests
{
    private static readonly NegocioDelMensaje Negocio = new(Datos.NegocioId, new CuentaWhatsapp(WebhookPayloads.PhoneNumberId, ""));
    private readonly IFiadoService _fiado = Substitute.For<IFiadoService>();
    private readonly IWhatsappContextService _contexto = Substitute.For<IWhatsappContextService>();
    private readonly SenderFalso _sender = new();
    private readonly FiadoBotService _bot;

    public FiadoBotServiceTests()
    {
        _contexto.EstaAutorizadoAsync(Datos.NegocioId, WebhookPayloads.Operador, Arg.Any<CancellationToken>()).Returns(true);
        _bot = new FiadoBotService(_fiado, _contexto, _sender, new MemoriaSesionBotStore(new RelojFijo()));
    }

    private Task<IReadOnlyList<EnvioWhatsapp>> Decir(string texto = "", string respuesta = "") => _bot.ProcesarAsync(Negocio, new MensajeEntrante { From = WebhookPayloads.Operador, Texto = texto, RespuestaId = respuesta });

    [Fact]
    public async Task Remitente_no_autorizado_recibe_un_aviso_y_nada_mas()
    {
        _contexto.EstaAutorizadoAsync(Datos.NegocioId, WebhookPayloads.Operador, Arg.Any<CancellationToken>()).Returns(false);

        var enviados = await Decir("hola");

        enviados.Should().ContainSingle().Which.Texto.Should().Contain("no está autorizado");
        await _fiado.DidNotReceiveWithAnyArgs().ListarDeudoresAsync(default);
    }

    [Fact]
    public async Task Un_saludo_muestra_el_menu_principal() => (await Decir("Hola")).Should().ContainSingle().Which.Tipo.Should().Be("list");

    [Fact]
    public async Task Flujo_completo_de_anotar_credito()
    {
        _fiado.BuscarClientesAsync(Datos.NegocioId, "María", Arg.Any<CancellationToken>()).Returns([new ClienteFiadoDto { Id = 5, Nombre = "María López", Saldo = 20m }]);
        _fiado.AgregarCargoAsync(Datos.NegocioId, 5, 50m, Arg.Any<string?>(), Arg.Any<CancellationToken>()).Returns(new ClienteFiadoDto { Id = 5, Nombre = "María López", Saldo = 70m });

        (await Decir(respuesta: "menu_fiado"))[0].Texto.Should().Contain("Nombre del cliente");
        (await Decir("María"))[0].Texto.Should().Contain("¿Cuánto");
        var final = await Decir("L. 50");

        final[0].Texto.Should().Contain("Anotado").And.Contain("70");
        await _fiado.Received(1).AgregarCargoAsync(Datos.NegocioId, 5, 50m, Arg.Any<string?>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Varias_coincidencias_ofrecen_una_lista_para_elegir()
    {
        _fiado.BuscarClientesAsync(Datos.NegocioId, "Ana", Arg.Any<CancellationToken>()).Returns([new ClienteFiadoDto { Id = 1, Nombre = "Ana A" }, new ClienteFiadoDto { Id = 2, Nombre = "Ana B" }]);

        await Decir(respuesta: "menu_saldo");
        var enviados = await Decir("Ana");

        enviados[0].Tipo.Should().Be("list");
        enviados[0].Texto.Should().Contain("2 clientes");
    }

    [Fact]
    public async Task Monto_ilegible_se_vuelve_a_pedir()
    {
        _fiado.BuscarClientesAsync(Datos.NegocioId, "María", Arg.Any<CancellationToken>()).Returns([new ClienteFiadoDto { Id = 5, Nombre = "María" }]);
        await Decir(respuesta: "menu_abono");
        await Decir("María");

        (await Decir("mucho"))[0].Texto.Should().Contain("No entendí el monto");
        await _fiado.DidNotReceiveWithAnyArgs().RegistrarAbonoAsync(default, default, default);
    }

    [Fact]
    public async Task Error_de_negocio_se_explica_al_operador()
    {
        _fiado.ObtenerSaldoAsync(Datos.NegocioId, 9, Arg.Any<CancellationToken>()).Returns<ClienteFiadoDto>(_ => throw new DomainException("Algo no cuadra."));

        var enviados = await Decir(respuesta: "fiar_9");

        enviados.Should().ContainSingle().Which.Texto.Should().Be("⚠️ Algo no cuadra.");
    }

    [Theory]
    [InlineData("50", 50)]
    [InlineData("L. 25.5", 25.5)]
    [InlineData("lps 1,000", 1000)]
    public void TryLeerMonto_acepta_formatos_comunes(string texto, decimal esperado)
    {
        FiadoBotService.TryLeerMonto(texto, out var monto).Should().BeTrue();
        monto.Should().Be(esperado);
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("0")]
    [InlineData("-5")]
    public void TryLeerMonto_rechaza_montos_invalidos(string texto) => FiadoBotService.TryLeerMonto(texto, out _).Should().BeFalse();
}
