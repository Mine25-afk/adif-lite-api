using Adif.Domain.Common;
using Adif.Domain.Entities;

namespace Adif.Domain.Fiado;

// Reparte un pago entre los créditos abiertos del más antiguo al más nuevo (FIFO).
public static class DistribucionAbono
{
    public static ResultadoAbono Aplicar(IEnumerable<Venta> creditosAbiertos, decimal monto, DateTime fecha, string comentario)
    {
        if (monto <= 0) throw new DomainException("El abono debe ser mayor a cero.");

        var restante = Dinero.Redondear(monto);
        var abonos = new List<Abono>();

        foreach (var venta in creditosAbiertos.Where(v => v.Saldo > 0).OrderBy(v => v.Fecha).ThenBy(v => v.Id))
        {
            if (restante <= 0) break;

            var aplicado = venta.AplicarAbono(restante);
            restante -= aplicado;
            abonos.Add(new Abono { NegocioId = venta.NegocioId, VentaId = venta.Id, Fecha = fecha, Monto = aplicado, Comentario = comentario });
        }

        return new ResultadoAbono(abonos.Sum(a => a.Monto), restante, abonos);
    }
}
