using System.Text.Json;
using Adif.Application.Common;
using Adif.Application.Dtos;
using Adif.Infrastructure.WhatsApp;
using Adif.UnitTests.Support;

namespace Adif.UnitTests.Infrastructure;

public class WhatsAppInfraTests
{
    [Fact]
    public void Botones_respetan_el_limite_de_WhatsApp_y_recortan_titulos()
    {
        var botones = Enumerable.Range(1, 5).Select(i => new BotonWhatsapp($"b{i}", "Un título demasiado largo para WhatsApp")).ToList();

        using var json = JsonDocument.Parse(JsonSerializer.Serialize(KapsoPayloads.Botones("504", "hola", botones)));
        var enviados = json.RootElement.GetProperty("interactive").GetProperty("action").GetProperty("buttons");

        enviados.GetArrayLength().Should().Be(KapsoPayloads.MaximoBotones);
        enviados[0].GetProperty("reply").GetProperty("title").GetString()!.Length.Should().Be(20);
    }

    [Fact]
    public void Deduplicador_rechaza_el_segundo_intento_del_mismo_id()
    {
        var deduplicador = new MemoriaDeduplicadorMensajes();

        deduplicador.EsNuevo("wamid.1").Should().BeTrue();
        deduplicador.EsNuevo("wamid.1").Should().BeFalse();
        deduplicador.EsNuevo("wamid.2").Should().BeTrue();
    }

    [Fact]
    public void Sesion_del_bot_expira_tras_30_minutos_de_inactividad()
    {
        var reloj = new RelojFijo();
        var sesiones = new MemoriaSesionBotStore(reloj);
        sesiones.Obtener(1, "504").Paso = "monto";

        reloj.Avanzar(TimeSpan.FromMinutes(10));
        sesiones.Obtener(1, "504").Paso.Should().Be("monto");

        reloj.Avanzar(TimeSpan.FromMinutes(31));
        sesiones.Obtener(1, "504").Paso.Should().BeEmpty();
    }

    [Theory]
    [InlineData(0, 0, 1, Paginacion.TamanoPorDefecto)]
    [InlineData(3, 500, 3, Paginacion.TamanoMaximo)]
    [InlineData(2, 15, 2, 15)]
    public void Paginacion_acota_pagina_y_tamano(int pagina, int tamano, int paginaEsperada, int tamanoEsperado) => Paginacion.Normalizar(pagina, tamano).Should().Be((paginaEsperada, tamanoEsperado));

    [Fact]
    public void PagedResult_calcula_total_de_paginas() => new PagedResult<int> { Items = [1, 2], Total = 41, TamanoPagina = 20 }.TotalPaginas.Should().Be(3);
}
