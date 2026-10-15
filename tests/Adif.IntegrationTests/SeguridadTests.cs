using System.Net;
using Adif.IntegrationTests.Support;

namespace Adif.IntegrationTests;

// Con el JwtBearer real de Firebase (sin auth de prueba).
public class SeguridadTests : IClassFixture<SeguridadTests.FactoryJwtReal>
{
    public sealed class FactoryJwtReal() : IDisposable
    {
        public ApiFactory Api { get; } = new(authDePrueba: false);
        public void Dispose() => Api.Dispose();
    }

    private readonly HttpClient _cliente;

    public SeguridadTests(FactoryJwtReal fixture) => _cliente = fixture.Api.CreateClient();

    [Theory]
    [InlineData("/api/v1/productos?negocioId=1")]
    [InlineData("/api/v1/ventas?negocioId=1")]
    [InlineData("/api/v1/negocios/mios")]
    [InlineData("/api/v1/licencias/dispositivos")]
    public async Task Endpoints_protegidos_sin_token_responden_401(string ruta)
    {
        var respuesta = await _cliente.GetAsync(ruta);

        respuesta.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        respuesta.Headers.WwwAuthenticate.Should().Contain(h => h.Scheme == "Bearer");
    }

    [Fact]
    public async Task Healthz_es_publico_y_reporta_la_base()
    {
        var respuesta = await _cliente.GetAsync("/healthz");

        respuesta.StatusCode.Should().Be(HttpStatusCode.OK);
        (await respuesta.Content.ReadAsStringAsync()).Should().Be("Healthy");
    }

    [Fact]
    public async Task Swagger_no_se_expone_fuera_de_Development() => (await _cliente.GetAsync("/swagger/v1/swagger.json")).StatusCode.Should().NotBe(HttpStatusCode.OK);
}
