// Renglón de una venta.
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Adif.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Adif.Domain.Entities;

[Table("ventas_detalles")]
[Index(nameof(NegocioId))]
[Index(nameof(VentaId))]
public class VentaDetalle : IPerteneceANegocio
{
    [Key, Column("id")] public int Id { get; set; }
    [Column("negocio_id")] public int NegocioId { get; set; }
    [Column("venta_id")] public int VentaId { get; set; }
    [Column("producto_id")] public int ProductoId { get; set; }
    [Column("precio", TypeName = "numeric(12,2)")] public decimal Precio { get; set; }
    [Column("costo", TypeName = "numeric(12,2)")] public decimal Costo { get; set; }
    [Column("cantidad", TypeName = "numeric(12,3)")] public decimal Cantidad { get; set; }

    [NotMapped] public decimal Importe => Dinero.Redondear(Precio * Cantidad);

    [ForeignKey(nameof(VentaId))]
    [InverseProperty(nameof(Entities.Venta.Detalles))]
    public virtual Venta? Venta { get; set; }
}
