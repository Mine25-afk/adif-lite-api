using Adif.Application.Dtos;
using FluentValidation;

namespace Adif.Application.Validators;

public sealed class BloqueoDispositivoValidator : AbstractValidator<BloqueoDispositivoDto>
{
    public BloqueoDispositivoValidator()
    {
        RuleFor(x => x.Telefono).NotEmpty().MaximumLength(100);
    }
}
