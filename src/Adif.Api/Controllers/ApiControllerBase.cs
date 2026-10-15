using System.Security.Claims;
using Adif.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Adif.Api.Controllers;

[ApiController]
[Authorize]
[Produces("application/json")]
public abstract class ApiControllerBase : ControllerBase
{
    // El uid sale siempre del token validado, nunca del body ni de la query.
    protected string Uid => User.FindFirstValue(FirebaseAuthenticationExtensions.ClaimUid) ?? User.FindFirstValue("sub") ?? throw new UnauthorizedAccessException("Token sin identificador de usuario.");
}
