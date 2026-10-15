using System.Globalization;
using Adif.Application.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Adif.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("es");
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.TryAddSingleton(TimeProvider.System);

        services.AddScoped<IAccesoNegocio, AccesoNegocio>();
        services.AddScoped<INegocioService, NegocioService>();
        services.AddScoped<IProductoService, ProductoService>();
        services.AddScoped<IVentaService, VentaService>();
        services.AddScoped<IFiadoService, FiadoService>();
        services.AddScoped<IWhatsappContextService, WhatsappContextService>();
        services.AddScoped<IFiadoBotService, FiadoBotService>();
        services.AddScoped<IWebhookKapsoService, WebhookKapsoService>();
        services.AddScoped<ILicenciaService, LicenciaService>();
        return services;
    }
}
