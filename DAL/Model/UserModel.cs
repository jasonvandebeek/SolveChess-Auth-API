
using SolveChess.Logic.Models;
using System.ComponentModel.DataAnnotations;

namespace SolveChess.DAL.Model;

public class UserModel : User
{

    [Key]
    public new string Id { get; set; } = null!;

}

