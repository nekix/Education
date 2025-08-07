using CarPark.Identity;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarPark.Controllers.Api.Controllers;

[Authorize(AppIdentityConst.ManagerPolicy)]
public class CsrfController : ApiBaseController
{
    private readonly IAntiforgery _antiforgery;

    public CsrfController(IAntiforgery antiforgery)
    {
        _antiforgery = antiforgery;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetTokenResponse))]
    [IgnoreAntiforgeryToken]
    public ActionResult<GetTokenResponse> GetToken()
    {
        AntiforgeryTokenSet token = _antiforgery.GetAndStoreTokens(HttpContext);

        GetTokenResponse response = new GetTokenResponse
        {
            Token = token.RequestToken
        };

        return Ok(response);
    }

    public class GetTokenResponse
    {
        public required string? Token { get; init; }
    }
}