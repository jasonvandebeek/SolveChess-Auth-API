
using SolveChess.Logic.DTO;
using SolveChess.Logic.Attributes;

namespace SolveChess.Logic.DAL;

public interface IAuthenticationDAL
{

    public UserDTO? GetUser(string email);
    public UserDTO CreateUser(string email, string? password, AuthType authType);

}
