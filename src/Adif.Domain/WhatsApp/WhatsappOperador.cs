// Teléfono autorizado a operar el fiado por WhatsApp en un negocio.
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Adif.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Adif.Domain.Entities;

[Table("whatsapp_operadores")]
[Index(nameof(NegocioId), nameof(Telefono))]
public class WhatsappOperador : IPerteneceANegocio
{
    [Key, Column("id")] public int Id { get; set; }
    [Column("negocio_id")] public int NegocioId { get; set; }
    [Column("telefono"), MaxLength(30)] public string Telefono { get; set; } = string.Empty;
    [Column("nombre"), MaxLength(80)] public string Nombre { get; set; } = string.Empty;
    [Column("activo")] public bool Activo { get; set; } = true;

    [ForeignKey(nameof(NegocioId))] public virtual Negocio? Negocio { get; set; }

    public static string NormalizarTelefono(string? telefono) => new((telefono ?? string.Empty).Where(char.IsDigit).ToArray());
}
