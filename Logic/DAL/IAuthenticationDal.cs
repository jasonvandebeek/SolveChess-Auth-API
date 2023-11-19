
using SolveChess.Logic.DTO;
using SolveChess.Logic.Attributes;

namespace SolveChess.Logic.DAL;

public interface IAuthenticationDal
{

    public UserDto? GetUser(string email);
    public UserDto CreateUser(string email, string? password, AuthType authType);

}
