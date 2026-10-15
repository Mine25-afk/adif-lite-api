// Dispositivo de un usuario con su licencia mensual de la app.
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Adif.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Adif.Domain.Entities;

[Table("adif_tbUsuarios")]
[PrimaryKey(nameof(Usuario), nameof(Telefono))]
[Index(nameof(Uid))]
public class LicenciaDispositivo
{
    public const int MaximoMesesPorCompra = 12;

    [Column("user_username"), MaxLength(50)] public string Usuario { get; set; } = string.Empty;
    [Column("logs_telefono"), MaxLength(100)] public string Telefono { get; set; } = string.Empty;
    [Column("user_bloqueado")] public bool Bloqueado { get; set; }
    [Column("user_google"), MaxLength(128)] public string Uid { get; set; } = string.Empty;
    [Column("user_mensualidad", TypeName = "timestamp without time zone")] public DateTime Mensualidad { get; set; } = Fechas.SinFecha;
    [Column("user_cantidad")] public int Cantidad { get; set; }

    public bool Vigente(DateTime hoy) => !Bloqueado && Mensualidad.Date >= hoy.Date;

    public void ExtenderMensualidad(int meses, DateTime hoy)
    {
        if (meses is < 1 or > MaximoMesesPorCompra) throw new DomainException($"Se pueden comprar entre 1 y {MaximoMesesPorCompra} meses.");

        var desde = Mensualidad.Date > hoy.Date ? Mensualidad.Date : hoy.Date;
        Mensualidad = desde.AddMonths(meses);
        Cantidad = meses;
    }
}
