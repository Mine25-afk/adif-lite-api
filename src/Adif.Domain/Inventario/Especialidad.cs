// Tipo de ítem: producto o servicio.
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Adif.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Adif.Domain.Entities;

[Table("especialidades")]
[Index(nameof(NegocioId))]
public class Especialidad : IPerteneceANegocio
{
    [Key, Column("id")] public int Id { get; set; }
    [Column("negocio_id")] public int NegocioId { get; set; }
    [Column("especialidad"), MaxLength(60)] public string Nombre { get; set; } = string.Empty;
}
