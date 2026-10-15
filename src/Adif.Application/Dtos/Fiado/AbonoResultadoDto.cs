// Resultado de aplicar un abono al fiado de un cliente.
namespace Adif.Application.Dtos;

public sealed class AbonoResultadoDto
{
    public decimal Aplicado { get; init; }
    public decimal Sobrante { get; init; }
    public decimal SaldoRestante { get; init; }
    public string Mensaje { get; init; } = string.Empty;
}
