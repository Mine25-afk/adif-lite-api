using Adif.Application.Abstractions.Persistence;
using Adif.Application.Dtos;
using Adif.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Adif.Infrastructure.Persistence.Repositories;

public sealed class SocioRepository(AdifDbContext db) : Repository<Socio>(db), ISocioRepository
{
    public Task<Socio?> ObtenerClienteAsync(int negocioId, int socioId, CancellationToken ct = default) => Clientes(negocioId).FirstOrDefaultAsync(s => s.Id == socioId, ct);

    public Task<Socio?> ObtenerClientePorNombreAsync(int negocioId, string nombre, CancellationToken ct = default)
    {
        var buscado = nombre.Trim().ToLower();
        return Clientes(negocioId).FirstOrDefaultAsync(s => s.Nombre.ToLower() == buscado, ct);
    }

    // Saldo por subconsulta correlacionada: una sola ida a la base en vez de N+1.
    public async Task<IReadOnlyList<ClienteFiadoDto>> BuscarClientesConSaldoAsync(int negocioId, string texto, int maximo, CancellationToken ct = default)
    {
        var buscado = texto.Trim().ToLower();
        var query = Clientes(negocioId);
        if (buscado.Length > 0) query = query.Where(s => s.Nombre.ToLower().Contains(buscado) || s.Telefono.Contains(buscado));

        return await query.AsNoTracking().OrderBy(s => s.Nombre).Take(maximo).Select(s => new ClienteFiadoDto
        {
            Id = s.Id,
            Nombre = s.Nombre,
            Telefono = s.Telefono,
            Saldo = Db.Ventas.Where(v => v.NegocioId == negocioId && v.SocioId == s.Id && v.EsCredito && v.Estado == EstadoVenta.Activa && v.Saldo > 0).Sum(v => (decimal?)v.Saldo) ?? 0m
        }).ToListAsync(ct);
    }

    private IQueryable<Socio> Clientes(int negocioId) => Set.Where(s => s.NegocioId == negocioId && !s.EsProveedor);
}
