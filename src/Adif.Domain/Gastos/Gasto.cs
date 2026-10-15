// Gasto operativo no ligado a inventario.
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Adif.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Adif.Domain.Entities;

[Table("gastos")]
[Index(nameof(NegocioId))]
public class Gasto : IPerteneceANegocio
{
    [Key, Column("id")] public int Id { get; set; }
    [Column("negocio_id")] public int NegocioId { get; set; }
    [Column("fecha", TypeName = "timestamp without time zone")] public DateTime Fecha { get; set; } = DateTime.Now;
    [Column("categoria"), MaxLength(60)] public string Categoria { get; set; } = string.Empty;
    [Column("descripcion")] public string Descripcion { get; set; } = string.Empty;
    [Column("monto", TypeName = "numeric(12,2)")] public decimal Monto { get; set; }
    [Column("metodo")] public MetodoPago Metodo { get; set; } = MetodoPago.Efectivo;
}
