using Adif.Application.Dtos;
using FluentValidation;

namespace Adif.Application.Validators;

public sealed class AjusteStockValidator : AbstractValidator<AjusteStockDto>
{
    public AjusteStockValidator()
    {
        RuleFor(x => x.NegocioId).GreaterThan(0);
        RuleFor(x => x.ProductoId).GreaterThan(0);
        RuleFor(x => x.Diferencia).NotEqual(0).WithMessage("El ajuste no puede ser cero.");
        RuleFor(x => x.Motivo).NotEmpty().MaximumLength(120);
    }
}
