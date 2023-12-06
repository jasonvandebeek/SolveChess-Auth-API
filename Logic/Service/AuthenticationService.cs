
using Newtonsoft.Json.Linq;
using SolveChess.Logic.Attributes;
using SolveChess.Logic.DAL;
using SolveChess.Logic.Models;
using SolveChess.Logic.Interfaces;
using SolveChess.Logic.ServiceInterfaces;
using SolveChess.Logic.Exceptions;

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
        try
        {
            var response = await _httpClient.GetAsync($"https://www.googleapis.com/oauth2/v3/userinfo?access_token={accessToken}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            JObject jsonObject = JObject.Parse(content);
            string? email = jsonObject["email"]?.ToString();
            if (email == null)
                return null;

            User? user = await _authenticationDal.GetUser(email);
            if (user == null)
            {
                user = new User()
                {
                    Id = GetNewUserId(),
                    Email = email,
                    Password = null,
                    AuthType = AuthType.GOOGLE
                };

                await _authenticationDal.CreateUser(user);
            };

            return _jwtProvider.GenerateToken(user.Id);
        }
        catch(Exception exception)
        {
            throw new AuthenticationException("An exception occured while authenticating with google: " + exception.Message);
        }
    }

    private static string GetNewUserId()
    {
        return Guid.NewGuid().ToString();
    }

}

