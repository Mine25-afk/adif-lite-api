// Compra a proveedor, de contado o a crédito.
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Adif.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Adif.Domain.Entities;

[Table("compras")]
[Index(nameof(NegocioId))]
public class Compra : IPerteneceANegocio
{
    [Key, Column("id")] public int Id { get; set; }
    [Column("negocio_id")] public int NegocioId { get; set; }
    [Column("socios_id")] public int SocioId { get; set; }
    [Column("fecha", TypeName = "timestamp without time zone")] public DateTime Fecha { get; set; } = DateTime.Now;
    [Column("fecha_vence", TypeName = "timestamp without time zone")] public DateTime FechaVence { get; set; } = Fechas.SinFecha;
    [Column("total", TypeName = "numeric(12,2)")] public decimal Total { get; set; }
    [Column("es_credito")] public bool EsCredito { get; set; }
    [Column("saldo", TypeName = "numeric(12,2)")] public decimal Saldo { get; set; }
    [Column("metodo")] public MetodoPago Metodo { get; set; } = MetodoPago.Efectivo;
    [Column("comentario")] public string Comentario { get; set; } = string.Empty;
    [Column("estado")] public int Estado { get; set; } = 1;
}
