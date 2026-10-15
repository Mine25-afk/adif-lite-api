using Adif.Application.Dtos;
using Adif.Application.Validators;
using FluentValidation.TestHelper;

namespace Adif.UnitTests.Validators;

public class OtrosValidatorsTests
{
    [Fact]
    public void Producto_con_mayoreo_mayor_que_el_precio_es_invalido() =>
        new CrearProductoValidator().TestValidate(new CrearProductoDto { NegocioId = 1, Nombre = "Azúcar", Precio = 10m, PrecioMayoreo = 12m }).ShouldHaveValidationErrorFor(x => x.PrecioMayoreo);

    [Fact]
    public void Producto_sin_nombre_es_invalido() => new CrearProductoValidator().TestValidate(new CrearProductoDto { NegocioId = 1 }).ShouldHaveValidationErrorFor(x => x.Nombre);

    [Fact]
    public void Negocio_exige_secreto_de_webhook_robusto()
    {
        var dto = new CrearNegocioDto { Nombre = "Pulpería", PhoneNumberId = "123456789", WebhookSecret = "corto" };

        new CrearNegocioValidator().TestValidate(dto).ShouldHaveValidationErrorFor(x => x.WebhookSecret);
    }

    [Fact]
    public void Negocio_valida_formato_del_phone_number_id_y_del_operador()
    {
        var dto = new CrearNegocioDto { Nombre = "Pulpería", PhoneNumberId = "abc", WebhookSecret = new string('s', 32), OperadorTelefono = "12" };

        var resultado = new CrearNegocioValidator().TestValidate(dto);

        resultado.ShouldHaveValidationErrorFor(x => x.PhoneNumberId);
        resultado.ShouldHaveValidationErrorFor(x => x.OperadorTelefono);
    }

    [Fact]
    public void Negocio_bien_formado_es_valido() =>
        new CrearNegocioValidator().TestValidate(new CrearNegocioDto { Nombre = "Pulpería", PhoneNumberId = "123456789", WebhookSecret = new string('s', 32), OperadorTelefono = "+50400000001" }).ShouldNotHaveAnyValidationErrors();

    [Fact]
    public void Cargo_de_fiado_sin_cliente_ni_nombre_es_invalido()
    {
        var resultado = new CargoFiadoValidator().TestValidate(new CargoFiadoRequest { PhoneNumberId = "123456789", From = "50400000001", Monto = 10m });

        resultado.ShouldHaveValidationErrorFor("cliente");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(2_000_000)]
    public void Cargo_de_fiado_con_monto_fuera_de_rango_es_invalido(decimal monto) =>
        new CargoFiadoValidator().TestValidate(new CargoFiadoRequest { PhoneNumberId = "123456789", From = "50400000001", ClienteId = 1, Monto = monto }).ShouldHaveValidationErrorFor("monto");

    [Fact]
    public void Abono_sin_remitente_es_invalido() =>
        new AbonoFiadoValidator().TestValidate(new AbonoFiadoRequest { PhoneNumberId = "123456789", ClienteId = 1, Monto = 5m }).ShouldHaveValidationErrorFor("from");

    [Theory]
    [InlineData(0)]
    [InlineData(13)]
    public void Mensualidad_fuera_de_rango_es_invalida(int meses) => new CompraMensualidadValidator().TestValidate(new CompraMensualidadDto { Telefono = "A1", Meses = meses }).ShouldHaveValidationErrorFor(x => x.Meses);

    [Fact]
    public void Ajuste_de_stock_en_cero_es_invalido() => new AjusteStockValidator().TestValidate(new AjusteStockDto { NegocioId = 1, ProductoId = 1, Motivo = "conteo" }).ShouldHaveValidationErrorFor(x => x.Diferencia);
}
