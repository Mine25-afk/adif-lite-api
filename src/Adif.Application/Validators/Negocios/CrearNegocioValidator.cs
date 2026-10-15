using Adif.Application.Dtos;
using FluentValidation;

namespace Adif.Application.Validators;

public sealed class CrearNegocioValidator : AbstractValidator<CrearNegocioDto>
{
    // Un secreto corto haría trivial falsificar la firma HMAC del webhook.
    public const int LargoMinimoSecreto = 16;

    public CrearNegocioValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Telefono).MaximumLength(30);
        RuleFor(x => x.Rtn).MaximumLength(20);
        RuleFor(x => x.PhoneNumberId).NotEmpty().SoloDigitos(40);
        RuleFor(x => x.DisplayNumber).MaximumLength(30);
        RuleFor(x => x.WabaId).MaximumLength(40);
        RuleFor(x => x.WebhookSecret).NotEmpty().MinimumLength(LargoMinimoSecreto).MaximumLength(120);
        RuleFor(x => x.KapsoApiKey).MaximumLength(120);
        RuleFor(x => x.OperadorTelefono).TelefonoE164().When(x => x.OperadorTelefono.Length > 0);
        RuleFor(x => x.OperadorNombre).MaximumLength(80);
    }
}
