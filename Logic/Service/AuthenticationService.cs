
using Newtonsoft.Json.Linq;
using SolveChess.Logic.Attributes;
using SolveChess.Logic.DAL;
using SolveChess.Logic.DTO;
using SolveChess.Logic.ServiceInterfaces;

namespace SolveChess.Logic.Service;

public class AuthenticationService : IAuthenticationService
{

    private readonly IAuthenticationDal _authenticationDAL;
    private readonly JwtProvider _jwtProvider;

    public AuthenticationService(IAuthenticationDal authenticaitonDAL, string jwtSecret)
    {
        _authenticationDAL = authenticaitonDAL;
        _jwtProvider = new JwtProvider(jwtSecret);
    }

    public async Task<string?> AuthenticateGoogle(string accessToken)
    {
        using var client = new HttpClient();
        var response = await client.GetAsync($"https://www.googleapis.com/oauth2/v3/userinfo?access_token={accessToken}");
        if (!response.IsSuccessStatusCode)
            return null;

        var content = await response.Content.ReadAsStringAsync();
        JObject jsonObject = JObject.Parse(content);
        string? email = jsonObject["email"]?.ToString();
        if(email == null) 
            return null;

        UserDto user = _authenticationDAL.GetUser(email) ?? _authenticationDAL.CreateUser(email, null, AuthType.GOOGLE);

        return _jwtProvider.GenerateToken(user.Id);
    }

}

