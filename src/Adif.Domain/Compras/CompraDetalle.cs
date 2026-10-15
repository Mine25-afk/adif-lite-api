// Renglón de una compra.
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Adif.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Adif.Domain.Entities;

[Table("compras_detalles")]
[Index(nameof(NegocioId))]
[Index(nameof(CompraId))]
public class CompraDetalle : IPerteneceANegocio
{
    [Key, Column("id")] public int Id { get; set; }
    [Column("negocio_id")] public int NegocioId { get; set; }
    [Column("compra_id")] public int CompraId { get; set; }
    [Column("producto_id")] public int ProductoId { get; set; }
    [Column("precio", TypeName = "numeric(12,2)")] public decimal Precio { get; set; }
    [Column("cantidad", TypeName = "numeric(12,3)")] public decimal Cantidad { get; set; }
}
