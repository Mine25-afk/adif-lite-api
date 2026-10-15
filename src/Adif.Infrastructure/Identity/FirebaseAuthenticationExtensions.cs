using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Adif.Infrastructure.Identity;

public static class FirebaseAuthenticationExtensions
{
    public const string ClaimUid = "user_id";

    // Valida el idToken de Firebase como JWT estándar: emisor, audiencia, vigencia y firma (JWKS de Google).
    public static IServiceCollection AddFirebaseAuthentication(this IServiceCollection services, IConfiguration configuracion)
    {
        services.AddOptions<FirebaseOptions>()
            .Bind(configuracion.GetSection("Firebase"))
            .Validate(o => !string.IsNullOrWhiteSpace(o.ProjectId), "Falta Firebase:ProjectId.")
            .ValidateOnStart();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme).Configure<IOptions<FirebaseOptions>>((jwt, firebase) =>
        {
            var projectId = firebase.Value.ProjectId;
            var emisor = $"https://securetoken.google.com/{projectId}";

            jwt.Authority = emisor;
            jwt.MapInboundClaims = false;
            jwt.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = emisor,
                ValidateAudience = true,
                ValidAudience = projectId,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.FromMinutes(2),
                NameClaimType = ClaimUid
            };
            jwt.Events = new JwtBearerEvents
            {
                // Firebase exige un `sub` no vacío; sin él el token no identifica a nadie.
                OnTokenValidated = ctx =>
                {
                    if (string.IsNullOrWhiteSpace(ctx.Principal?.FindFirst("sub")?.Value)) ctx.Fail("Token sin sujeto.");
                    return Task.CompletedTask;
                }
            };
        });

        return services;
    }
}
