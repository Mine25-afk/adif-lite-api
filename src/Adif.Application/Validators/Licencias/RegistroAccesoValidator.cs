using Adif.Application.Dtos;
using FluentValidation;

namespace Adif.Application.Validators;

public sealed class RegistroAccesoValidator : AbstractValidator<RegistroAccesoDto>
{
    public RegistroAccesoValidator()
    {
        RuleFor(x => x.Usuario).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Telefono).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Accion).NotEmpty().MaximumLength(100);
    }
}
