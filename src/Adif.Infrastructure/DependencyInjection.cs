using Adif.Application.Abstractions.Persistence;
using Adif.Application.Abstractions.Services;
using Adif.Infrastructure.Identity;
using Adif.Infrastructure.Persistence;
using Adif.Infrastructure.Persistence.Repositories;
using Adif.Infrastructure.Storage;
using Adif.Infrastructure.WhatsApp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Adif.Infrastructure;

public static class DependencyInjection
{
    public const string CadenaPostgres = "Postgres";

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuracion)
    {
        services.AddDbContext<AdifDbContext>((sp, o) => o.UseNpgsql(sp.GetRequiredService<IConfiguration>().GetConnectionString(CadenaPostgres)));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IProductoRepository, ProductoRepository>();
        services.AddScoped<IVentaRepository, VentaRepository>();
        services.AddScoped<ISocioRepository, SocioRepository>();
        services.AddScoped<INegocioRepository, NegocioRepository>();
        services.AddScoped<INegocioWhatsappRepository, NegocioWhatsappRepository>();
        services.AddScoped<ILicenciaDispositivoRepository, LicenciaDispositivoRepository>();

        services.AddOptions<BlobStorageOptions>().Configure(o =>
        {
            o.ConnectionString = configuracion.GetConnectionString("Blob") ?? string.Empty;
            o.Container = configuracion["Blob:Container"] is { Length: > 0 } contenedor ? contenedor : o.Container;
        });
        services.AddSingleton<IImageStorage, AzureBlobImageStorage>();

        services.AddOptions<KapsoOptions>().Bind(configuracion.GetSection("Kapso"));
        services.AddHttpClient<IWhatsAppSender, KapsoWhatsAppSender>((sp, http) => http.Timeout = TimeSpan.FromSeconds(sp.GetRequiredService<IOptions<KapsoOptions>>().Value.TimeoutSegundos));
        services.TryAddSingleton(TimeProvider.System);
        services.AddSingleton<ISesionBotStore, MemoriaSesionBotStore>();
        services.AddSingleton<IDeduplicadorMensajes, MemoriaDeduplicadorMensajes>();

        services.AddFirebaseAuthentication(configuracion);
        return services;
    }
}
