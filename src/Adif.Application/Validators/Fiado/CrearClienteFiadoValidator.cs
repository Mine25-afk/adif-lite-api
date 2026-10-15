using Adif.Application.Dtos;
using FluentValidation;

namespace Adif.Application.Validators;

public sealed class CrearClienteFiadoValidator : AbstractValidator<CrearClienteFiadoRequest>
{
    public CrearClienteFiadoValidator()
    {
        Include(new FiadoBaseValidator());
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(120).OverridePropertyName("nombre");
        RuleFor(x => x.Telefono).MaximumLength(30).OverridePropertyName("telefono");
    }
}
