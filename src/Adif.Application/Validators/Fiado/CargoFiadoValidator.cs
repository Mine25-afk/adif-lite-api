using Adif.Application.Dtos;
using FluentValidation;

namespace Adif.Application.Validators;

public sealed class CargoFiadoValidator : AbstractValidator<CargoFiadoRequest>
{
    public CargoFiadoValidator()
    {
        Include(new FiadoBaseValidator());
        RuleFor(x => x.Monto).Monto().OverridePropertyName("monto");
        RuleFor(x => x.Descripcion).MaximumLength(200).OverridePropertyName("descripcion");
        RuleFor(x => x).Must(x => x.ClienteId > 0 || !string.IsNullOrWhiteSpace(x.Nombre)).WithName("cliente").WithMessage("Debe indicar cliente_id o nombre.");
    }
}
