// Usuario operador de la app dentro de una pulpería.
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Adif.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Adif.Domain.Entities;

[Table("usuarios")]
[Index(nameof(NegocioId))]
public class Usuario : IPerteneceANegocio
{
    [Key, Column("id")] public int Id { get; set; }
    [Column("negocio_id")] public int NegocioId { get; set; }
    [Column("usuario"), MaxLength(80)] public string Nombre { get; set; } = string.Empty;
    [Column("bloqueado")] public bool Bloqueado { get; set; }
    [Column("google"), MaxLength(128)] public string Google { get; set; } = string.Empty;
    [Column("mensualidad"), MaxLength(40)] public string Mensualidad { get; set; } = string.Empty;
    [Column("cantidad")] public int Cantidad { get; set; }
}
