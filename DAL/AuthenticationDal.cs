
using Microsoft.EntityFrameworkCore;
using SolveChess.DAL.Model;
using SolveChess.Logic.Attributes;
using SolveChess.Logic.DAL;
using SolveChess.Logic.Models;

namespace SolveChess.DAL;

public class AuthenticationDal : IAuthenticationDal
{

    private readonly AppDbContext _dbContext;

    public AuthenticationDal(DbContextOptions<AppDbContext> options)
    {
        _dbContext = new AppDbContext(options);
    }

    public async Task CreateUser(User user)
    {
        var userModel = new UserModel()
        {
            Id = user.Id,
            Email = user.Email,
            Password = user.Password,
            AuthType = user.AuthType
        };

        _dbContext.User.Add(userModel);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<User?> GetUser(string email)
    {
        var userModel = await _dbContext.User
            .Where(u => u.Email == email)
            .FirstOrDefaultAsync();

        if (userModel == null)
            return null;

        var user = new User()
        {
            Id = userModel.Id,
            Email = userModel.Email,
            Password = userModel.Password,
            AuthType = userModel.AuthType
        };

        return user;
    }

}
