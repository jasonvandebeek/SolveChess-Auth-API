
namespace SolveChess.Logic.Interfaces;

public interface IJwtProvider
{

    public string GenerateToken(string userId);

}
