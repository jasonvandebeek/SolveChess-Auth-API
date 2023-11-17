using Microsoft.AspNetCore.Mvc;
using SolveChess.API.Models;
using SolveChess.Logic.ServiceInterfaces;

namespace SolveChess.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{

    private readonly IAuthenticationService _authenticationService;

    public AuthController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpPost("google")]
    public async Task<IActionResult> GoogleLogin([FromBody] AccessTokenModel request)
    {
        string? jwtToken = await _authenticationService.AuthenticateGoogle(request.AccessToken);
        if (jwtToken == null)
            return Unauthorized();

        var cookieOptions = new CookieOptions
        {
            Expires = DateTime.Now.AddHours(1),
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict
        };

        Response.Cookies.Append("AccessToken", jwtToken, cookieOptions);

        return Ok();
    }

}
