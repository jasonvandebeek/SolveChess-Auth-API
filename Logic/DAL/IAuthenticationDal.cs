
using SolveChess.Logic.Models;

namespace SolveChess.Logic.DAL;

public interface IAuthenticationDal
{

    public Task<User?> GetUserWithEmail(string email);
    public Task<User?> GetUserWithId(string userId);
    public Task CreateUser(User user);

}
