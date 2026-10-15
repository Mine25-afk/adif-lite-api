// Configuración clave-valor por negocio.
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Adif.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Adif.Domain.Entities;

[Table("objetos")]
[Index(nameof(NegocioId))]
public class ConfiguracionNegocio : IPerteneceANegocio
{
    [Key, Column("id")] public int Id { get; set; }
    [Column("negocio_id")] public int NegocioId { get; set; }
    [Column("objeto"), MaxLength(60)] public string Clave { get; set; } = string.Empty;
    [Column("estado"), MaxLength(120)] public string Valor { get; set; } = string.Empty;
}
