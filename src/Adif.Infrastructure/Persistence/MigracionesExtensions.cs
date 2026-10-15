using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Adif.Infrastructure.Persistence;

public static class MigracionesExtensions
{
    // Reintenta porque en docker-compose la API puede arrancar antes que PostgreSQL.
    public static async Task AplicarMigracionesAsync(this IServiceProvider servicios, int intentos = 10, CancellationToken ct = default)
    {
        using var scope = servicios.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AdifDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("Migraciones");

        for (var intento = 1; ; intento++)
        {
            try
            {
                await db.Database.MigrateAsync(ct);
                logger.LogInformation("Migraciones aplicadas");
                return;
            }
            catch (Exception ex) when (intento < intentos)
            {
                logger.LogWarning(ex, "Base no lista (intento {Intento}/{Total}); reintentando", intento, intentos);
                await Task.Delay(TimeSpan.FromSeconds(3), ct);
            }
        }
    }
}
