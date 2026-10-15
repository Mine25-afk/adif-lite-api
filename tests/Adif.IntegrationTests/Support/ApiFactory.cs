using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Adif.Domain.Entities;
using Adif.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Adif.IntegrationTests.Support;

// API real en memoria: EF InMemory en lugar de PostgreSQL y, opcionalmente, auth de prueba en lugar de Firebase.
public sealed class ApiFactory(bool authDePrueba = true, string toolsApiKey = ApiFactory.ToolsApiKey, string entorno = "Testing") : WebApplicationFactory<Program>
{
    public const string ToolsApiKey = "clave-de-herramientas-de-prueba";
    public const string UidDueno = "uid-dueno";
    public const string UidOtro = "uid-otro";
    public const string PhoneNumberId = "100200300400";
    public const string SecretoWebhook = "secreto-webhook-de-prueba";
    public const string Operador = "50400000001";

    public static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web) { Converters = { new JsonStringEnumConverter() } };

    private readonly string _baseDatos = $"adif-{Guid.NewGuid()}";

    public int NegocioId { get; private set; }
    public int NegocioAjenoId { get; private set; }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment(entorno);
        builder.UseSetting("Database:AplicarMigracionesAlIniciar", "false");
        builder.UseSetting("Firebase:ProjectId", "adif-test");
        builder.UseSetting("Kapso:ToolsApiKey", toolsApiKey);
        builder.UseSetting("Kapso:ProjectApiKey", "");
        builder.UseSetting("ConnectionStrings:Postgres", "Host=no-usada");

        builder.ConfigureTestServices(services =>
        {
            var deEf = services.Where(d => d.ServiceType == typeof(DbContextOptions<AdifDbContext>) || d.ServiceType == typeof(DbContextOptions)
                || (d.ServiceType.IsGenericType && d.ServiceType.GetGenericTypeDefinition() == typeof(IDbContextOptionsConfiguration<>))).ToList();
            foreach (var descriptor in deEf) services.Remove(descriptor);
            services.AddDbContext<AdifDbContext>(o => o.UseInMemoryDatabase(_baseDatos));

            if (authDePrueba)
                services.AddAuthentication(o =>
                {
                    o.DefaultAuthenticateScheme = TestAuthHandler.Esquema;
                    o.DefaultChallengeScheme = TestAuthHandler.Esquema;
                }).AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.Esquema, _ => { });
        });
    }

    public HttpClient ClienteDe(string uid)
    {
        var cliente = CreateClient();
        cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TestAuthHandler.Esquema, uid);
        return cliente;
    }

    public async Task SembrarAsync()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AdifDbContext>();

        var propio = new Negocio { Nombre = "Pulpería La Esquina", Propietario = UidDueno };
        propio.Numeros.Add(new NegocioWhatsapp { PhoneNumberId = PhoneNumberId, WebhookSecret = SecretoWebhook });
        var ajeno = new Negocio { Nombre = "Pulpería Ajena", Propietario = UidOtro };
        db.Negocios.AddRange(propio, ajeno);
        await db.SaveChangesAsync();

        db.OperadoresWhatsapp.Add(new WhatsappOperador { NegocioId = propio.Id, Telefono = Operador });
        var cliente = new Socio { NegocioId = propio.Id, Nombre = "María López" };
        db.Socios.Add(cliente);
        await db.SaveChangesAsync();

        db.Ventas.Add(new Venta { NegocioId = propio.Id, SocioId = cliente.Id, EsCredito = true, Total = 80m, Saldo = 80m });
        await db.SaveChangesAsync();

        NegocioId = propio.Id;
        NegocioAjenoId = ajeno.Id;
    }
}
