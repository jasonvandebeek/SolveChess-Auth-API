using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SolveChess.API.Exceptions;
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

    [Authorize]
    [HttpGet("userId")]
    public IActionResult GetUserId()
    {
        string userId = GetUserIdFromCookies();
        return Ok(userId);
    }

    [HttpPost("google-login")]
    public async Task<IActionResult> GoogleLogin([FromBody] AccessTokenModel request)
    {
        string? jwtToken = await _authenticationService.AuthenticateGoogle(request.AccessToken);
        if (jwtToken == null)
            return Unauthorized();

        AddAccessToken(jwtToken, DateTime.Now.AddHours(1));

        return Ok();
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        AddAccessToken("", DateTime.Now.AddYears(-1));

        return Ok();
    }

    [HttpGet("user/{id}")]
    public async Task<IActionResult> DoesUserExist(string id)
    {
        if (!await _authenticationService.DoesUserExist(id))
            return NotFound();

        return Ok(id);
    }

    private void AddAccessToken(string accessToken, DateTime expirationData)
    {
        var cookieOptions = new CookieOptions
        {
            Expires = DateTime.Now.AddYears(-1),
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None
        };

        Response.Cookies.Append("AccessToken", accessToken, cookieOptions);
    }

    private string GetUserIdFromCookies()
    {
        var userId = HttpContext.User.FindFirst("Id")?.Value ?? throw new InvalidJwtTokenException();
        return userId;
    }

}
