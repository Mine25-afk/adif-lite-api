// Pago parcial sobre una venta o compra a crédito.
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Adif.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Adif.Domain.Entities;

[Table("abonos")]
[Index(nameof(NegocioId))]
[Index(nameof(VentaId))]
[Index(nameof(CompraId))]
public class Abono : IPerteneceANegocio
{
    [Key, Column("id")] public int Id { get; set; }
    [Column("negocio_id")] public int NegocioId { get; set; }
    [Column("venta_id")] public int VentaId { get; set; }
    [Column("compra_id")] public int CompraId { get; set; }
    [Column("fecha", TypeName = "timestamp without time zone")] public DateTime Fecha { get; set; } = DateTime.Now;
    [Column("monto", TypeName = "numeric(12,2)")] public decimal Monto { get; set; }
    [Column("metodo")] public MetodoPago Metodo { get; set; } = MetodoPago.Efectivo;
    [Column("comentario")] public string Comentario { get; set; } = string.Empty;
}
