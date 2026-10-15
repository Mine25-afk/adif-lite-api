// Categoría de producto.
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Adif.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Adif.Domain.Entities;

[Table("categorias")]
[Index(nameof(NegocioId))]
public class Categoria : IPerteneceANegocio
{
    [Key, Column("id")] public int Id { get; set; }
    [Column("negocio_id")] public int NegocioId { get; set; }
    [Column("nombre"), MaxLength(80)] public string Nombre { get; set; } = string.Empty;
    [Column("icono"), MaxLength(60)] public string Icono { get; set; } = string.Empty;
}
