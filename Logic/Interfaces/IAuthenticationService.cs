
namespace SolveChess.Logic.ServiceInterfaces;

public interface IAuthenticationService
{

    public Task<string?> AuthenticateGoogle(string accessToken);
    public Task<bool> DoesUserExist(string userId);

}
