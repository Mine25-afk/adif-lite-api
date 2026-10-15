// Venta nueva de contado o a crédito.
using Adif.Domain.Common;

namespace Adif.Application.Dtos;

public sealed class RegistrarVentaDto
{
    public int NegocioId { get; set; }
    public int SocioId { get; set; }
    public bool EsCredito { get; set; }
    public int DiasCredito { get; set; }
    public bool AplicaIsv { get; set; }
    public MetodoPago Metodo { get; set; } = MetodoPago.Efectivo;
    public string Comentario { get; set; } = string.Empty;
    public List<LineaVentaDto> Lineas { get; set; } = [];
}
