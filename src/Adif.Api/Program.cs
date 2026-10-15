using System.Text.Json.Serialization;
using Adif.Api.Extensions;
using Adif.Api.Filters;
using Adif.Api.Middleware;
using Adif.Application;
using Adif.Infrastructure;
using Adif.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Serilog;
using Serilog.Formatting.Compact;

const string PoliticaCors = "adif";

var builder = WebApplication.CreateBuilder(args);

// Logs estructurados a stdout (12-factor): legibles en desarrollo, JSON compacto en el resto.
builder.Services.AddSerilog((servicios, log) =>
{
    log.ReadFrom.Configuration(builder.Configuration).ReadFrom.Services(servicios).Enrich.FromLogContext();
    if (builder.Environment.IsDevelopment()) log.WriteTo.Console();
    else log.WriteTo.Console(new RenderedCompactJsonFormatter());
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers(o => o.Filters.Add<ValidacionFilter>()).AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddProblemDetails(o => o.CustomizeProblemDetails = c => c.ProblemDetails.Extensions["traceId"] = c.HttpContext.TraceIdentifier);
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Seguro por defecto: todo endpoint exige token salvo que declare [AllowAnonymous].
builder.Services.AddAuthorizationBuilder().SetFallbackPolicy(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());
builder.Services.AddHealthChecks().AddDbContextCheck<AdifDbContext>("postgres");

var origenes = builder.Configuration.GetSection("Cors:OrigenesPermitidos").Get<string[]>() ?? [];
builder.Services.AddCors(o => o.AddPolicy(PoliticaCors, p => p.WithOrigins(origenes).AllowAnyHeader().AllowAnyMethod()));

var detrasDeProxy = builder.Configuration.GetValue<bool>("ReverseProxy:Habilitado");
if (detrasDeProxy)
{
    builder.Services.Configure<ForwardedHeadersOptions>(o =>
    {
        // El proxy (Caddy/nginx) vive en la red interna del contenedor y no tiene IP fija.
        o.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        o.KnownIPNetworks.Clear();
        o.KnownProxies.Clear();
    });
}

if (builder.Environment.IsDevelopment()) builder.Services.AddSwaggerDocumentation();

var app = builder.Build();

if (app.Configuration.GetValue<bool>("Database:AplicarMigracionesAlIniciar")) await app.Services.AplicarMigracionesAsync();

if (detrasDeProxy) app.UseForwardedHeaders();

// CORS antes del manejador de errores: si no, el navegador ve un error de red en vez del ProblemDetails.
app.UseCors(PoliticaCors);
app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(o => o.SwaggerEndpoint("/swagger/v1/swagger.json", "ADIF Lite API v1"));
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/healthz").AllowAnonymous();

await app.RunAsync();

public partial class Program;
