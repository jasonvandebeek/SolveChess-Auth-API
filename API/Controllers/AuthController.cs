using Microsoft.AspNetCore.Authorization;
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

    [HttpGet("userId")]
    public IActionResult GetUserId()
    {
        string? userId = HttpContext.User.FindFirst("Id")?.Value;
        if (userId == null)
            return Ok(null);

        return Ok(userId);
    }

    [HttpPost("google-login")]
    public async Task<IActionResult> GoogleLogin([FromBody] AccessTokenModel request)
    {
        if (request.AccessToken == null)
            return BadRequest();

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
