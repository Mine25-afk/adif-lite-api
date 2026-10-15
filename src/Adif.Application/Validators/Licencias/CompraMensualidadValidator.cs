using Adif.Application.Dtos;
using Adif.Domain.Entities;
using FluentValidation;

namespace Adif.Application.Validators;

public sealed class CompraMensualidadValidator : AbstractValidator<CompraMensualidadDto>
{
    public CompraMensualidadValidator()
    {
        RuleFor(x => x.Telefono).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Meses).InclusiveBetween(1, LicenciaDispositivo.MaximoMesesPorCompra);
    }
}
