using System.Net;
using System.Net.Http.Json;
using Adif.Application.Common;
using Adif.Application.Dtos;
using Adif.IntegrationTests.Support;
using Microsoft.AspNetCore.Mvc;

namespace Adif.IntegrationTests;

public class InventarioYVentasTests : IAsyncLifetime
{
    private readonly ApiFactory _api = new();
    private HttpClient _dueno = null!;

    public async Task InitializeAsync()
    {
        await _api.SembrarAsync();
        _dueno = _api.ClienteDe(ApiFactory.UidDueno);
    }

    public Task DisposeAsync()
    {
        _api.Dispose();
        return Task.CompletedTask;
    }

    private async Task<ProductoDto> CrearProducto(string nombre, decimal precio, decimal stock)
    {
        var respuesta = await _dueno.PostAsJsonAsync("/api/v1/productos", new CrearProductoDto { NegocioId = _api.NegocioId, Nombre = nombre, Precio = precio, Stock = stock }, ApiFactory.Json);
        respuesta.StatusCode.Should().Be(HttpStatusCode.Created);
        return (await respuesta.Content.ReadFromJsonAsync<ProductoDto>(ApiFactory.Json))!;
    }

    [Fact]
    public async Task Crear_y_listar_productos_paginados()
    {
        await CrearProducto("Arroz 1 lb", 12m, 40m);
        await CrearProducto("Frijoles 1 lb", 18m, 25m);

        var pagina = await _dueno.GetFromJsonAsync<PagedResult<ProductoDto>>($"/api/v1/productos?negocioId={_api.NegocioId}&buscar=arroz&tamano=10", ApiFactory.Json);

        pagina!.Total.Should().Be(1);
        pagina.Items.Should().ContainSingle().Which.Nombre.Should().Be("Arroz 1 lb");
    }

    [Fact]
    public async Task Registrar_venta_descuenta_el_inventario()
    {
        var producto = await CrearProducto("Aceite 750 ml", 45m, 10m);

        var respuesta = await _dueno.PostAsJsonAsync("/api/v1/ventas", new RegistrarVentaDto { NegocioId = _api.NegocioId, AplicaIsv = true, Lineas = [new LineaVentaDto { ProductoId = producto.Id, Cantidad = 2 }] }, ApiFactory.Json);

        respuesta.StatusCode.Should().Be(HttpStatusCode.Created);
        var venta = (await respuesta.Content.ReadFromJsonAsync<VentaDto>(ApiFactory.Json))!;
        venta.Total.Should().Be(103.50m);

        var actualizado = await _dueno.GetFromJsonAsync<ProductoDto>($"/api/v1/productos/detalle?negocioId={_api.NegocioId}&id={producto.Id}", ApiFactory.Json);
        actualizado!.Stock.Should().Be(8m);
    }

    [Fact]
    public async Task Vender_mas_de_lo_que_hay_responde_422_y_no_toca_el_stock()
    {
        var producto = await CrearProducto("Gaseosa 2 L", 40m, 1m);

        var respuesta = await _dueno.PostAsJsonAsync("/api/v1/ventas", new RegistrarVentaDto { NegocioId = _api.NegocioId, Lineas = [new LineaVentaDto { ProductoId = producto.Id, Cantidad = 3 }] }, ApiFactory.Json);

        respuesta.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        respuesta.Content.Headers.ContentType!.MediaType.Should().Be("application/problem+json");
        (await _dueno.GetFromJsonAsync<ProductoDto>($"/api/v1/productos/detalle?negocioId={_api.NegocioId}&id={producto.Id}", ApiFactory.Json))!.Stock.Should().Be(1m);
    }

    [Fact]
    public async Task Operar_un_negocio_ajeno_responde_403()
    {
        var respuesta = await _dueno.GetAsync($"/api/v1/productos?negocioId={_api.NegocioAjenoId}");

        respuesta.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Datos_invalidos_responden_400_con_ValidationProblemDetails()
    {
        var respuesta = await _dueno.PostAsJsonAsync("/api/v1/productos", new CrearProductoDto { NegocioId = _api.NegocioId, Nombre = "", Precio = -1m }, ApiFactory.Json);

        respuesta.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problema = await respuesta.Content.ReadFromJsonAsync<ValidationProblemDetails>(ApiFactory.Json);
        problema!.Errors.Should().ContainKeys("Nombre", "Precio");
    }
}
