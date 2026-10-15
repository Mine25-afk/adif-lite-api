using Adif.Application.Dtos;
using FluentValidation;

namespace Adif.Application.Validators;

public sealed class RegistrarVentaValidator : AbstractValidator<RegistrarVentaDto>
{
    public const int MaximoLineas = 100;
    public const int MaximoDiasCredito = 365;

    public RegistrarVentaValidator()
    {
        RuleFor(x => x.NegocioId).GreaterThan(0);
        RuleFor(x => x.SocioId).GreaterThanOrEqualTo(0);
        RuleFor(x => x.SocioId).GreaterThan(0).When(x => x.EsCredito).WithMessage("Una venta a crédito necesita un cliente.");
        RuleFor(x => x.DiasCredito).InclusiveBetween(0, MaximoDiasCredito);
        RuleFor(x => x.DiasCredito).Equal(0).When(x => !x.EsCredito).WithMessage("Los días de crédito solo aplican a ventas a crédito.");
        RuleFor(x => x.Metodo).IsInEnum();
        RuleFor(x => x.Comentario).MaximumLength(500);
        RuleFor(x => x.Lineas).NotEmpty().WithMessage("La venta debe tener al menos un producto.");
        RuleFor(x => x.Lineas.Count).LessThanOrEqualTo(MaximoLineas).WithName("Lineas");
        RuleForEach(x => x.Lineas).ChildRules(linea =>
        {
            linea.RuleFor(l => l.ProductoId).GreaterThan(0);
            linea.RuleFor(l => l.Cantidad).GreaterThan(0).LessThanOrEqualTo(100_000);
        });
    }
}
