
using SolveChess.Logic.Attributes;

namespace SolveChess.Logic.DTO;

public class UserDTO
{

    public string Id { get; set; }
    public string Email { get; set; }
    public string? Password { get; set; }
    public AuthType AuthType { get; set; }

}

