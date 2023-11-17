
using SolveChess.Logic.Attributes;
using System.ComponentModel.DataAnnotations;

namespace SolveChess.DAL.Model;

public class User
{

    [Key]
    public string Id { get; set; }
    public string Email { get; set; }
    public string? Password { get; set; }
    public AuthType AuthType { get; set; }

}

