
using Newtonsoft.Json.Linq;
using SolveChess.Logic.Attributes;
using SolveChess.Logic.DAL;
using SolveChess.Logic.DTO;
using SolveChess.Logic.Interfaces;
using SolveChess.Logic.ServiceInterfaces;

namespace SolveChess.Logic.Service;

public class AuthenticationService : IAuthenticationService
{

    private readonly IAuthenticationDal _authenticationDal;
    private readonly IJwtProvider _jwtProvider;
    private readonly HttpClient _httpClient;

    public AuthenticationService(IAuthenticationDal authenticationDal, IJwtProvider jwtProvider, HttpClient httpClient)
    {
        _authenticationDal = authenticationDal;
        _jwtProvider = jwtProvider;
        _httpClient = httpClient;
    }

    public async Task<string?> AuthenticateGoogle(string accessToken)
    {
        var response = await _httpClient.GetAsync($"https://www.googleapis.com/oauth2/v3/userinfo?access_token={accessToken}");
        if (!response.IsSuccessStatusCode)
            return null;

        var content = await response.Content.ReadAsStringAsync();
        JObject jsonObject = JObject.Parse(content);
        string? email = jsonObject["email"]?.ToString();
        if(email == null) 
            return null;

        UserDto user = _authenticationDal.GetUser(email) ?? _authenticationDal.CreateUser(email, null, AuthType.GOOGLE);

        return _jwtProvider.GenerateToken(user.Id);
    }

}

