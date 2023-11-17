
using Microsoft.EntityFrameworkCore;
using SolveChess.DAL.Model;
using SolveChess.Logic.Attributes;
using SolveChess.Logic.DAL;
using SolveChess.Logic.DTO;

namespace SolveChess.DAL;

public class AuthenticationDAL : IAuthenticationDAL
{

    private readonly AppDbContext _dbContext;

    public AuthenticationDAL(DbContextOptions<AppDbContext> options)
    {
        _dbContext = new AppDbContext(options);
    }

    public UserDTO CreateUser(string email, string? password, AuthType authType)
    {
        string guid = Guid.NewGuid().ToString();

        var user = new User() { 
            Id = guid,
            Email = email, 
            Password = password, 
            AuthType = authType
        };

        _dbContext.User.Add(user);
        _dbContext.SaveChanges();

        var userDTO = new UserDTO() 
        { 
            Id = user.Id,
            Email = user.Email,
            Password = user.Password,
            AuthType = user.AuthType
        };

        return userDTO;
    }

    public UserDTO? GetUser(string email)
    {
        var user = _dbContext.User
            .Where(u => u.Email == email)
            .FirstOrDefault();

        if (user == null)
            return null;

        var userDTO = new UserDTO()
        {
            Id = user.Id,
            Email = user.Email,
            Password = user.Password,
            AuthType = user.AuthType
        };

        return userDTO;
    }

}
