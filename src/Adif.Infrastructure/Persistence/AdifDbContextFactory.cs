using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Adif.Infrastructure.Persistence;

// Solo para `dotnet ef`: la cadena llega por variable de entorno, nunca escrita aquí.
public sealed class AdifDbContextFactory : IDesignTimeDbContextFactory<AdifDbContext>
{
    public AdifDbContext CreateDbContext(string[] args)
    {
        var cadena = Environment.GetEnvironmentVariable("ADIF_DESIGN_CONNECTION") ?? "Host=localhost;Port=5432;Database=adif_lite;Username=adif";
        return new AdifDbContext(new DbContextOptionsBuilder<AdifDbContext>().UseNpgsql(cadena).Options);
    }
}
