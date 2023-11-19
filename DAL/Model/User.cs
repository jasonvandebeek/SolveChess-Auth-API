
using SolveChess.Logic.Attributes;
using System.ComponentModel.DataAnnotations;

namespace SolveChess.DAL.Model;

public class User
{

    [Key]
    public string Id { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Password { get; set; }
    public AuthType AuthType { get; set; }

}

