using System.ComponentModel.DataAnnotations;

namespace GameInventoryApi.Contracts;

public class LoginRequest
{
    [Required]
    public string Nickname { get; set; } = string.Empty;
}
