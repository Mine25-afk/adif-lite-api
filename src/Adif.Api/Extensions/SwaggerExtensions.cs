using Adif.Api.Security;
using Microsoft.OpenApi;

namespace Adif.Api.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "ADIF Lite API", Version = "v1", Description = "Backend multi-tenant para pulperías: inventario, ventas, fiado por WhatsApp y licencias." });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme { Type = SecuritySchemeType.Http, Scheme = "bearer", BearerFormat = "JWT", Description = "idToken de Firebase (sin el prefijo Bearer)." });
            c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme { Type = SecuritySchemeType.ApiKey, In = ParameterLocation.Header, Name = ToolsApiKeyAttribute.Header, Description = "Clave de las herramientas de fiado (Kapso)." });
            c.AddSecurityRequirement(documento => new OpenApiSecurityRequirement { [new OpenApiSecuritySchemeReference("Bearer", documento)] = [] });
        });
        return services;
    }
}
