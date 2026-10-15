using Adif.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Adif.Infrastructure.Persistence;

// El mapeo vive en atributos de cada entidad; aquí solo lo que un atributo no expresa.
public sealed class AdifDbContext(DbContextOptions<AdifDbContext> options) : DbContext(options)
{
    public DbSet<Negocio> Negocios => Set<Negocio>();
    public DbSet<Socio> Socios => Set<Socio>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<ConfiguracionNegocio> Configuraciones => Set<ConfiguracionNegocio>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Especialidad> Especialidades => Set<Especialidad>();
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Venta> Ventas => Set<Venta>();
    public DbSet<VentaDetalle> VentasDetalles => Set<VentaDetalle>();
    public DbSet<Abono> Abonos => Set<Abono>();
    public DbSet<Compra> Compras => Set<Compra>();
    public DbSet<CompraDetalle> ComprasDetalles => Set<CompraDetalle>();
    public DbSet<CajaMovimiento> CajaMovimientos => Set<CajaMovimiento>();
    public DbSet<Gasto> Gastos => Set<Gasto>();
    public DbSet<Evento> Eventos => Set<Evento>();
    public DbSet<NegocioWhatsapp> NumerosWhatsapp => Set<NegocioWhatsapp>();
    public DbSet<WhatsappOperador> OperadoresWhatsapp => Set<WhatsappOperador>();
    public DbSet<LicenciaDispositivo> LicenciaDispositivos => Set<LicenciaDispositivo>();
    public DbSet<LicenciaLog> LicenciaLogs => Set<LicenciaLog>();
    public DbSet<CodigoActivacion> CodigosActivacion => Set<CodigoActivacion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Canjear un código dos veces a la vez debe fallar en el segundo UPDATE.
        modelBuilder.Entity<CodigoActivacion>().Property(c => c.Usado).IsConcurrencyToken();

        // Borrar un negocio no debe arrastrar en silencio sus números ni operadores.
        modelBuilder.Entity<NegocioWhatsapp>().HasOne(w => w.Negocio).WithMany(n => n.Numeros).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<WhatsappOperador>().HasOne(o => o.Negocio).WithMany().OnDelete(DeleteBehavior.Restrict);

        AplicarDefaultsSinNull(modelBuilder);
    }

    // Toda columna de texto o número lleva DEFAULT en la base: la app nunca escribe NULL.
    private static void AplicarDefaultsSinNull(ModelBuilder modelBuilder)
    {
        foreach (var propiedad in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetProperties()))
        {
            if (propiedad.IsKey() || propiedad.IsForeignKey() || propiedad.ValueGenerated != Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.Never) continue;

            var tipo = Nullable.GetUnderlyingType(propiedad.ClrType) ?? propiedad.ClrType;
            if (tipo == typeof(string)) propiedad.SetDefaultValue(string.Empty);
            else if (tipo == typeof(decimal)) propiedad.SetDefaultValue(0m);
            else if (tipo == typeof(int)) propiedad.SetDefaultValue(0);
        }
    }
}
