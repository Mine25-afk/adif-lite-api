using Adif.Application.Services;

namespace Adif.UnitTests.Application;

public class FirmaWebhookTests
{
    private const string Secreto = "secreto-de-prueba-123";
    private const string Cuerpo = "{\"hola\":\"mundo\"}";

    [Fact]
    public void Firma_correcta_es_valida_sin_importar_mayusculas()
    {
        var firma = FirmaWebhook.Calcular(Cuerpo, Secreto);

        FirmaWebhook.EsValida(Cuerpo, Secreto, firma).Should().BeTrue();
        FirmaWebhook.EsValida(Cuerpo, Secreto, firma.ToUpperInvariant()).Should().BeTrue();
    }

    [Fact]
    public void Cuerpo_alterado_invalida_la_firma() => FirmaWebhook.EsValida(Cuerpo + " ", Secreto, FirmaWebhook.Calcular(Cuerpo, Secreto)).Should().BeFalse();

    [Fact]
    public void Otro_secreto_invalida_la_firma() => FirmaWebhook.EsValida(Cuerpo, "otro-secreto-distinto", FirmaWebhook.Calcular(Cuerpo, Secreto)).Should().BeFalse();

    [Theory]
    [InlineData(null, "abc")]
    [InlineData("", "abc")]
    [InlineData(Secreto, null)]
    [InlineData(Secreto, "")]
    public void Sin_secreto_o_sin_firma_siempre_rechaza(string? secreto, string? firma) => FirmaWebhook.EsValida(Cuerpo, secreto, firma).Should().BeFalse();
}
