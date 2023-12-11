
using SolveChess.Logic.Attributes;
using System.ComponentModel.DataAnnotations;

namespace SolveChess.DAL.Model;

public class UserModel
{

    [Key]
    public string Id { get; set; } = null!;
    [Required]
    public string Email { get; set; } = null!;

    public string? Password { get; set; }

    [Required]
    public AuthType AuthType { get; set; }

}

