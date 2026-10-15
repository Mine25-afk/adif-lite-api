using Adif.Application.Dtos;
using Adif.Application.Validators;
using FluentValidation.TestHelper;

namespace Adif.UnitTests.Validators;

public class RegistrarVentaValidatorTests
{
    private readonly RegistrarVentaValidator _validador = new();

    private static RegistrarVentaDto Valida() => new() { NegocioId = 1, Lineas = [new LineaVentaDto { ProductoId = 3, Cantidad = 2 }] };

    [Fact]
    public void Venta_de_contado_bien_formada_es_valida() => _validador.TestValidate(Valida()).ShouldNotHaveAnyValidationErrors();

    [Fact]
    public void Credito_sin_cliente_es_invalido()
    {
        var dto = Valida();
        dto.EsCredito = true;

        _validador.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.SocioId);
    }

    [Fact]
    public void Dias_de_credito_en_venta_de_contado_son_invalidos()
    {
        var dto = Valida();
        dto.DiasCredito = 30;

        _validador.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.DiasCredito);
    }

    [Fact]
    public void Sin_lineas_es_invalida() => _validador.TestValidate(new RegistrarVentaDto { NegocioId = 1 }).ShouldHaveValidationErrorFor(x => x.Lineas);

    [Fact]
    public void Linea_con_cantidad_cero_es_invalida()
    {
        var dto = Valida();
        dto.Lineas[0].Cantidad = 0;

        _validador.TestValidate(dto).ShouldHaveValidationErrorFor("Lineas[0].Cantidad");
    }
}
