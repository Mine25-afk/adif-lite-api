using Adif.Application.Dtos;
using FluentValidation;

namespace Adif.Application.Validators;

// Reglas comunes de toda herramienta de fiado; internal para no registrarse sola en DI.
internal sealed class FiadoBaseValidator : AbstractValidator<FiadoBaseRequest>
{
    public FiadoBaseValidator()
    {
        RuleFor(x => x.PhoneNumberId).NotEmpty().SoloDigitos(40).OverridePropertyName("phone_number_id");
        RuleFor(x => x.From).NotEmpty().TelefonoE164().OverridePropertyName("from");
    }
}
