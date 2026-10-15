// Ajuste manual de inventario (positivo entra, negativo sale).
namespace Adif.Application.Dtos;

public sealed class AjusteStockDto
{
    public int NegocioId { get; set; }
    public int ProductoId { get; set; }
    public decimal Diferencia { get; set; }
    public string Motivo { get; set; } = string.Empty;
}
