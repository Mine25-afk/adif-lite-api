// Tenant raíz: una pulpería.
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Adif.Domain.Entities;

[Table("negocios")]
[Index(nameof(Propietario))]
public class Negocio
{
    [Key, Column("id")] public int Id { get; set; }
    [Column("nombre"), MaxLength(120)] public string Nombre { get; set; } = string.Empty;
    [Column("propietario"), MaxLength(128)] public string Propietario { get; set; } = string.Empty;
    [Column("telefono"), MaxLength(30)] public string Telefono { get; set; } = string.Empty;
    [Column("direccion")] public string Direccion { get; set; } = string.Empty;
    [Column("rtn"), MaxLength(20)] public string Rtn { get; set; } = string.Empty;
    [Column("activo")] public bool Activo { get; set; } = true;
    [Column("created_at", TypeName = "timestamp without time zone")] public DateTime CreadoEn { get; set; } = DateTime.Now;

    [InverseProperty(nameof(NegocioWhatsapp.Negocio))]
    public virtual ICollection<NegocioWhatsapp> Numeros { get; set; } = new List<NegocioWhatsapp>();
}
