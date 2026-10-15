using Adif.Application.Dtos;
using FluentValidation;

namespace Adif.Application.Validators;

public sealed class AgregarOperadorValidator : AbstractValidator<AgregarOperadorDto>
{
    public AgregarOperadorValidator()
    {
        RuleFor(x => x.NegocioId).GreaterThan(0);
        RuleFor(x => x.Telefono).NotEmpty().TelefonoE164();
        RuleFor(x => x.Nombre).MaximumLength(80);
    }
}
