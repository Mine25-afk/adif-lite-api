// Movimiento de efectivo de la caja diaria.
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Adif.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Adif.Domain.Entities;

[Table("caja_movimientos")]
[Index(nameof(NegocioId))]
public class CajaMovimiento : IPerteneceANegocio
{
    [Key, Column("id")] public int Id { get; set; }
    [Column("negocio_id")] public int NegocioId { get; set; }
    [Column("fecha", TypeName = "timestamp without time zone")] public DateTime Fecha { get; set; } = DateTime.Now;
    [Column("tipo")] public TipoMovimientoCaja Tipo { get; set; }
    [Column("monto", TypeName = "numeric(12,2)")] public decimal Monto { get; set; }
    [Column("concepto"), MaxLength(120)] public string Concepto { get; set; } = string.Empty;
    [Column("usuario"), MaxLength(80)] public string Usuario { get; set; } = string.Empty;
    [Column("saldo_resultante", TypeName = "numeric(12,2)")] public decimal SaldoResultante { get; set; }
}
