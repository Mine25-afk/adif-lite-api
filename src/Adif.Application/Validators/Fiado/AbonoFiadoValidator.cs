using Adif.Application.Dtos;
using FluentValidation;

namespace Adif.Application.Validators;

public sealed class AbonoFiadoValidator : AbstractValidator<AbonoFiadoRequest>
{
    public AbonoFiadoValidator()
    {
        Include(new FiadoBaseValidator());
        RuleFor(x => x.Monto).Monto().OverridePropertyName("monto");
        RuleFor(x => x).Must(x => x.ClienteId > 0 || !string.IsNullOrWhiteSpace(x.Nombre)).WithName("cliente").WithMessage("Debe indicar cliente_id o nombre.");
    }
}
