using Adif.Application.Dtos;
using FluentValidation;

namespace Adif.Application.Validators;

public sealed class ActualizarProductoValidator : AbstractValidator<ActualizarProductoDto>
{
    public ActualizarProductoValidator()
    {
        RuleFor(x => x.NegocioId).GreaterThan(0);
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Precio).GreaterThanOrEqualTo(0).LessThanOrEqualTo(ReglasComunes.MontoMaximo);
        RuleFor(x => x.PrecioMayoreo).GreaterThanOrEqualTo(0).LessThanOrEqualTo(x => x.Precio).WithMessage("El precio de mayoreo no puede ser mayor que el precio normal.");
        RuleFor(x => x.Costo).GreaterThanOrEqualTo(0);
        RuleFor(x => x.StockMinimo).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Unidad).NotEmpty().MaximumLength(20);
        RuleFor(x => x.CodigoBarra).MaximumLength(40);
        RuleFor(x => x.CategoriaId).GreaterThanOrEqualTo(0);
    }
}
