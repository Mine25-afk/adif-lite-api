using Adif.Api.Security;
using Adif.Application.Dtos;
using Adif.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Adif.Api.Controllers.V1;

// Herramientas que invoca el agente de Kapso: sin usuario Firebase, se autentican con X-API-Key.
[ApiController]
[AllowAnonymous]
[ToolsApiKey]
[Produces("application/json")]
[Route(ApiRoutes.V1 + "/fiado")]
public sealed class FiadoController(IFiadoService fiado, IWhatsappContextService contexto) : ControllerBase
{
    [HttpPost("clientes")]
    public async Task<ActionResult<ClienteFiadoDto>> CrearCliente(CrearClienteFiadoRequest req, CancellationToken ct)
    {
        var negocioId = await contexto.ResolverOperadorAsync(req.PhoneNumberId, req.From, ct);
        return Ok(await fiado.CrearClienteAsync(negocioId, req.Nombre, req.Telefono, ct));
    }

    [HttpGet("clientes/buscar")]
    public async Task<ActionResult<IReadOnlyList<ClienteFiadoDto>>> BuscarClientes([FromQuery(Name = "phone_number_id")] string phoneNumberId, [FromQuery(Name = "from")] string from, string? texto, CancellationToken ct)
    {
        var negocioId = await contexto.ResolverOperadorAsync(phoneNumberId, from, ct);
        return Ok(await fiado.BuscarClientesAsync(negocioId, texto, ct));
    }

    [HttpGet("clientes/saldo")]
    public async Task<ActionResult<ClienteFiadoDto>> Saldo([FromQuery(Name = "phone_number_id")] string phoneNumberId, [FromQuery(Name = "from")] string from, [FromQuery(Name = "cliente_id")] int clienteId, string? nombre, CancellationToken ct)
    {
        var negocioId = await contexto.ResolverOperadorAsync(phoneNumberId, from, ct);
        var socioId = await fiado.ResolverClienteAsync(negocioId, clienteId, nombre, ct);
        return Ok(await fiado.ObtenerSaldoAsync(negocioId, socioId, ct));
    }

    [HttpPost("cargos")]
    public async Task<ActionResult<ClienteFiadoDto>> AgregarCargo(CargoFiadoRequest req, CancellationToken ct)
    {
        var negocioId = await contexto.ResolverOperadorAsync(req.PhoneNumberId, req.From, ct);
        var socioId = await fiado.ResolverClienteAsync(negocioId, req.ClienteId, req.Nombre, ct);
        return Ok(await fiado.AgregarCargoAsync(negocioId, socioId, req.Monto, req.Descripcion, ct));
    }

    [HttpPost("abonos")]
    public async Task<ActionResult<AbonoResultadoDto>> RegistrarAbono(AbonoFiadoRequest req, CancellationToken ct)
    {
        var negocioId = await contexto.ResolverOperadorAsync(req.PhoneNumberId, req.From, ct);
        var socioId = await fiado.ResolverClienteAsync(negocioId, req.ClienteId, req.Nombre, ct);
        return Ok(await fiado.RegistrarAbonoAsync(negocioId, socioId, req.Monto, ct));
    }

    [HttpGet("deudores")]
    public async Task<ActionResult<IReadOnlyList<DeudorDto>>> Deudores([FromQuery(Name = "phone_number_id")] string phoneNumberId, [FromQuery(Name = "from")] string from, CancellationToken ct)
    {
        var negocioId = await contexto.ResolverOperadorAsync(phoneNumberId, from, ct);
        return Ok(await fiado.ListarDeudoresAsync(negocioId, ct));
    }
}
