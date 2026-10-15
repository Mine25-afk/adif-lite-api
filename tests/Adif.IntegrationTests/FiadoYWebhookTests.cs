using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Adif.Application.Dtos;
using Adif.Application.Services;
using Adif.IntegrationTests.Support;

namespace Adif.IntegrationTests;

public class FiadoYWebhookTests
{
    private static string RutaDeudores => $"/api/v1/fiado/deudores?phone_number_id={ApiFactory.PhoneNumberId}&from={ApiFactory.Operador}";

    private static async Task<ApiFactory> Levantar(string toolsApiKey = ApiFactory.ToolsApiKey)
    {
        var api = new ApiFactory(toolsApiKey: toolsApiKey);
        await api.SembrarAsync();
        return api;
    }

    [Fact]
    public async Task Herramientas_de_fiado_con_api_key_valida_listan_deudores()
    {
        using var api = await Levantar();
        var cliente = api.CreateClient();
        cliente.DefaultRequestHeaders.Add("X-API-Key", ApiFactory.ToolsApiKey);

        var deudores = await cliente.GetFromJsonAsync<List<DeudorDto>>(RutaDeudores, ApiFactory.Json);

        deudores.Should().ContainSingle().Which.Saldo.Should().Be(80m);
    }

    [Fact]
    public async Task Api_key_incorrecta_responde_401()
    {
        using var api = await Levantar();
        var cliente = api.CreateClient();
        cliente.DefaultRequestHeaders.Add("X-API-Key", "otra-clave");

        (await cliente.GetAsync(RutaDeudores)).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Sin_api_key_configurada_se_rechaza_todo_aunque_manden_header()
    {
        using var api = await Levantar(toolsApiKey: "");
        var cliente = api.CreateClient();
        cliente.DefaultRequestHeaders.Add("X-API-Key", "");

        (await cliente.GetAsync(RutaDeudores)).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Abono_por_herramienta_reduce_el_saldo()
    {
        using var api = await Levantar();
        var cliente = api.CreateClient();
        cliente.DefaultRequestHeaders.Add("X-API-Key", ApiFactory.ToolsApiKey);

        var respuesta = await cliente.PostAsJsonAsync("/api/v1/fiado/abonos", new AbonoFiadoRequest { PhoneNumberId = ApiFactory.PhoneNumberId, From = ApiFactory.Operador, Nombre = "María", Monto = 30m }, ApiFactory.Json);

        respuesta.StatusCode.Should().Be(HttpStatusCode.OK);
        (await respuesta.Content.ReadFromJsonAsync<AbonoResultadoDto>(ApiFactory.Json))!.SaldoRestante.Should().Be(50m);
    }

    [Fact]
    public async Task Webhook_con_firma_invalida_responde_401()
    {
        using var api = await Levantar();
        var cuerpo = Payload("hola");

        var respuesta = await api.CreateClient().SendAsync(Webhook(cuerpo, "firma-falsa"));

        respuesta.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Webhook_firmado_es_procesado_por_el_bot()
    {
        using var api = await Levantar();
        var cuerpo = Payload("hola");

        var respuesta = await api.CreateClient().SendAsync(Webhook(cuerpo, FirmaWebhook.Calcular(cuerpo, ApiFactory.SecretoWebhook)));

        respuesta.StatusCode.Should().Be(HttpStatusCode.OK);
        (await respuesta.Content.ReadAsStringAsync()).Should().Contain("\"procesado\":true");
    }

    private static string Payload(string texto) => JsonSerializer.Serialize(new
    {
        @event = "whatsapp.message.received",
        data = new { phone_number_id = ApiFactory.PhoneNumberId, message = new { id = $"wamid.{Guid.NewGuid()}", from = ApiFactory.Operador, text = new { body = texto } } }
    });

    private static HttpRequestMessage Webhook(string cuerpo, string firma)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/whatsapp/webhook") { Content = new StringContent(cuerpo, Encoding.UTF8, "application/json") };
        request.Headers.Add("X-Webhook-Signature", firma);
        return request;
    }
}
