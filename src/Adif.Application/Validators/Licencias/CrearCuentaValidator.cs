using Adif.Application.Dtos;
using FluentValidation;

namespace Adif.Application.Validators;

public sealed class CrearCuentaValidator : AbstractValidator<CrearCuentaDto>
{
    public CrearCuentaValidator()
    {
        RuleFor(x => x.Usuario).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Telefono).NotEmpty().MaximumLength(100);
    }
}
