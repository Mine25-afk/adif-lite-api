// Bitácora de accesos y compras de licencia por dispositivo.
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Adif.Domain.Entities;

[Table("adif_tbLogs")]
[PrimaryKey(nameof(Usuario), nameof(Fecha), nameof(Telefono))]
public class LicenciaLog
{
    [Column("user_username"), MaxLength(128)] public string Usuario { get; set; } = string.Empty;
    [Column("logs_fecha", TypeName = "timestamp without time zone")] public DateTime Fecha { get; set; } = DateTime.Now;
    [Column("logs_telefono"), MaxLength(100)] public string Telefono { get; set; } = string.Empty;
    [Column("logs_accion"), MaxLength(100)] public string Accion { get; set; } = string.Empty;
}
