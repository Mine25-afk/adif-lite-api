// Producto del inventario con sus reglas de stock y precio.
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Adif.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Adif.Domain.Entities;

[Table("productos")]
[Index(nameof(NegocioId))]
[Index(nameof(NegocioId), nameof(CodigoBarra))]
public class Producto : IPerteneceANegocio
{
    [Key, Column("id")] public int Id { get; set; }
    [Column("negocio_id")] public int NegocioId { get; set; }
    [Column("nombre"), MaxLength(120)] public string Nombre { get; set; } = string.Empty;
    [Column("precio", TypeName = "numeric(12,2)")] public decimal Precio { get; set; }
    [Column("costo", TypeName = "numeric(12,2)")] public decimal Costo { get; set; }
    [Column("stock", TypeName = "numeric(12,3)")] public decimal Stock { get; set; }
    [Column("cateid")] public int CategoriaId { get; set; }
    [Column("notas")] public string Notas { get; set; } = string.Empty;
    [Column("activo")] public bool Activo { get; set; } = true;
    [Column("img_blob"), MaxLength(200)] public string ImagenBlob { get; set; } = string.Empty;
    [Column("espeid")] public int EspecialidadId { get; set; }
    [Column("codigo_barra"), MaxLength(40)] public string CodigoBarra { get; set; } = string.Empty;
    [Column("unidad"), MaxLength(20)] public string Unidad { get; set; } = "unidad";
    [Column("precio_mayoreo", TypeName = "numeric(12,2)")] public decimal PrecioMayoreo { get; set; }
    [Column("stock_minimo", TypeName = "numeric(12,3)")] public decimal StockMinimo { get; set; }

    [NotMapped] public bool StockBajo => StockMinimo > 0 && Stock <= StockMinimo;

    public decimal PrecioPara(bool mayoreo) => mayoreo && PrecioMayoreo > 0 ? PrecioMayoreo : Precio;

    public void DescontarStock(decimal cantidad)
    {
        if (cantidad <= 0) throw new DomainException("La cantidad a descontar debe ser mayor a cero.");
        if (!Activo) throw new DomainException($"El producto '{Nombre}' está inactivo.");
        if (Stock < cantidad) throw new DomainException($"Stock insuficiente de '{Nombre}': hay {Stock:0.###} y se pidieron {cantidad:0.###}.");
        Stock -= cantidad;
    }

    public void ReponerStock(decimal cantidad)
    {
        if (cantidad <= 0) throw new DomainException("La cantidad a reponer debe ser mayor a cero.");
        Stock += cantidad;
    }

    public void AjustarStock(decimal diferencia)
    {
        if (diferencia == 0) throw new DomainException("El ajuste no puede ser cero.");
        if (Stock + diferencia < 0) throw new DomainException($"El ajuste dejaría el stock de '{Nombre}' en negativo.");
        Stock += diferencia;
    }
}
