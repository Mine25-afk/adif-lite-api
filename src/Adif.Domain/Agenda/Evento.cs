// Recordatorio de la agenda del negocio.
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Adif.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Adif.Domain.Entities;

[Table("eventos")]
[Index(nameof(NegocioId))]
public class Evento : IPerteneceANegocio
{
    [Key, Column("id")] public int Id { get; set; }
    [Column("negocio_id")] public int NegocioId { get; set; }
    [Column("titulo"), MaxLength(120)] public string Titulo { get; set; } = string.Empty;
    [Column("descripcion")] public string Descripcion { get; set; } = string.Empty;
    [Column("fecha", TypeName = "timestamp without time zone")] public DateTime Fecha { get; set; } = DateTime.Now;
    [Column("hora"), MaxLength(10)] public string Hora { get; set; } = string.Empty;
    [Column("created_at", TypeName = "timestamp without time zone")] public DateTime CreadoEn { get; set; } = DateTime.Now;
}
