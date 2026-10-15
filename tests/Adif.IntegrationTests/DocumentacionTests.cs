using System.Net;
using Adif.IntegrationTests.Support;

namespace Adif.IntegrationTests;

public class DocumentacionTests
{
    [Fact]
    public async Task En_Development_se_genera_el_documento_OpenAPI_v1()
    {
        using var api = new ApiFactory(entorno: "Development");

        var respuesta = await api.CreateClient().GetAsync("/swagger/v1/swagger.json");

        respuesta.StatusCode.Should().Be(HttpStatusCode.OK);
        var documento = await respuesta.Content.ReadAsStringAsync();
        documento.Should().Contain("/api/v1/ventas").And.Contain("/api/v1/fiado/abonos").And.Contain("bearer");
    }
}
