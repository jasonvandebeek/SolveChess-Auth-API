
using SolveChess.Logic.Attributes;

namespace SolveChess.Logic.DTO;

public class UserDto
{

    public string Id { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Password { get; set; }
    public AuthType AuthType { get; set; }

}

