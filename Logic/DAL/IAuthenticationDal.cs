
using SolveChess.Logic.Models;

namespace SolveChess.Logic.DAL;

public interface IAuthenticationDal
{

    public Task<User?> GetUser(string email);
    public Task CreateUser(User user);

}
