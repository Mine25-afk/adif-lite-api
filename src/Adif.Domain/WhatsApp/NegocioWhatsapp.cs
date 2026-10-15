// Número de WhatsApp Business ligado a un negocio y sus credenciales de Kapso.
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Adif.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Adif.Domain.Entities;

[Table("negocio_whatsapp")]
[Index(nameof(PhoneNumberId), IsUnique = true)]
[Index(nameof(NegocioId))]
public class NegocioWhatsapp : IPerteneceANegocio
{
    [Key, Column("id")] public int Id { get; set; }
    [Column("negocio_id")] public int NegocioId { get; set; }
    [Column("phone_number_id"), MaxLength(40)] public string PhoneNumberId { get; set; } = string.Empty;
    [Column("display_number"), MaxLength(30)] public string DisplayNumber { get; set; } = string.Empty;
    [Column("waba_id"), MaxLength(40)] public string WabaId { get; set; } = string.Empty;
    [Column("webhook_secret"), MaxLength(120)] public string WebhookSecret { get; set; } = string.Empty;
    [Column("kapso_api_key"), MaxLength(120)] public string KapsoApiKey { get; set; } = string.Empty;
    [Column("activo")] public bool Activo { get; set; } = true;
    [Column("created_at", TypeName = "timestamp without time zone")] public DateTime CreadoEn { get; set; } = DateTime.Now;

    [ForeignKey(nameof(NegocioId))]
    [InverseProperty(nameof(Entities.Negocio.Numeros))]
    public virtual Negocio? Negocio { get; set; }
}
