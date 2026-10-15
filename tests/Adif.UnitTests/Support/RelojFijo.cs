namespace Adif.UnitTests.Support;

// Reloj determinístico en UTC; se puede adelantar para probar expiraciones.
public sealed class RelojFijo(DateTime ahora) : TimeProvider
{
    public static readonly DateTime Hoy = new(2026, 3, 15, 10, 0, 0);

    public RelojFijo() : this(Hoy) { }

    public DateTime Ahora { get; private set; } = ahora;

    public void Avanzar(TimeSpan tiempo) => Ahora = Ahora.Add(tiempo);

    public override TimeZoneInfo LocalTimeZone => TimeZoneInfo.Utc;
    public override DateTimeOffset GetUtcNow() => new(DateTime.SpecifyKind(Ahora, DateTimeKind.Utc));
}
