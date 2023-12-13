
using Microsoft.EntityFrameworkCore;
using SolveChess.DAL.Model;
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
        _dbContext.User.Add(user);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<User?> GetUserWithEmail(string email)
    {
        var user = await _dbContext.User.FirstOrDefaultAsync(u => u.Email == email);
        return user;
    }

    public async Task<User?> GetUserWithId(string userId)
    {
        var user = await _dbContext.User.FirstOrDefaultAsync(u  => u.Id == userId);
        return user;
    }

}
