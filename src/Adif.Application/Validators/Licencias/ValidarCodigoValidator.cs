using Adif.Application.Dtos;
using FluentValidation;

namespace Adif.Application.Validators;

public sealed class ValidarCodigoValidator : AbstractValidator<ValidarCodigoDto>
{
    public ValidarCodigoValidator()
    {
        RuleFor(x => x.Codigo).NotEmpty().MaximumLength(60);
    }
}
