using FluentValidation;

namespace Adif.Application.Validators;

// Reglas reutilizadas por varios validadores.
internal static class ReglasComunes
{
    public const decimal MontoMaximo = 1_000_000m;

    public static IRuleBuilderOptions<T, string> TelefonoE164<T>(this IRuleBuilder<T, string> regla) => regla.Must(t => DigitosValidos(t, 8, 15)).WithMessage("'{PropertyName}' debe tener entre 8 y 15 dígitos (formato internacional, sin espacios).");

    public static IRuleBuilderOptions<T, string> SoloDigitos<T>(this IRuleBuilder<T, string> regla, int maximo) => regla.Must(t => DigitosValidos(t, 1, maximo)).WithMessage($"'{{PropertyName}}' solo admite dígitos (máximo {maximo}).");

    public static IRuleBuilderOptions<T, decimal> Monto<T>(this IRuleBuilder<T, decimal> regla) => regla.GreaterThan(0).LessThanOrEqualTo(MontoMaximo);

    private static bool DigitosValidos(string? valor, int minimo, int maximo)
    {
        var limpio = (valor ?? string.Empty).Trim().TrimStart('+');
        return limpio.Length >= minimo && limpio.Length <= maximo && limpio.All(char.IsDigit);
    }
}
