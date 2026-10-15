using Adif.Application.Abstractions.Persistence;
using Adif.Application.Dtos;
using Adif.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Adif.Infrastructure.Persistence.Repositories;

public sealed class VentaRepository(AdifDbContext db) : Repository<Venta>(db), IVentaRepository
{
    public Task<Venta?> ObtenerConDetallesAsync(int negocioId, int ventaId, CancellationToken ct = default) => Set.Include(v => v.Detalles).FirstOrDefaultAsync(v => v.NegocioId == negocioId && v.Id == ventaId, ct);

    public async Task<IReadOnlyList<Venta>> ListarCreditosAbiertosAsync(int negocioId, int socioId, CancellationToken ct = default) =>
        await CreditosAbiertos(negocioId).Where(v => v.SocioId == socioId).OrderBy(v => v.Fecha).ThenBy(v => v.Id).ToListAsync(ct);

    public async Task<decimal> CalcularSaldoClienteAsync(int negocioId, int socioId, CancellationToken ct = default) =>
        await CreditosAbiertos(negocioId).Where(v => v.SocioId == socioId).SumAsync(v => (decimal?)v.Saldo, ct) ?? 0m;

    public async Task<IReadOnlyList<DeudorDto>> ListarDeudoresAsync(int negocioId, CancellationToken ct = default)
    {
        var saldos = CreditosAbiertos(negocioId).GroupBy(v => v.SocioId).Select(g => new { SocioId = g.Key, Saldo = g.Sum(v => v.Saldo) });
        var deudores = from s in saldos
                       join socio in Db.Socios on s.SocioId equals socio.Id
                       where socio.NegocioId == negocioId
                       select new DeudorDto { Id = socio.Id, Nombre = socio.Nombre, Telefono = socio.Telefono, Saldo = s.Saldo };
        return await deudores.OrderByDescending(d => d.Saldo).ToListAsync(ct);
    }

    private IQueryable<Venta> CreditosAbiertos(int negocioId) => Set.Where(v => v.NegocioId == negocioId && v.EsCredito && v.Estado == EstadoVenta.Activa && v.Saldo > 0);
}
