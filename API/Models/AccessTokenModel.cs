using System.ComponentModel.DataAnnotations;

namespace SolveChess.API.Models;

public class AccessTokenModel
{

    [Required]
    public string AccessToken { get; set; } = null!;

}
