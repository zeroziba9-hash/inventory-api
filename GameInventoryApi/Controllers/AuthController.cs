using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GameInventoryApi.Contracts;
using GameInventoryApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace GameInventoryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IConfiguration config, GameDbContext db) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Nickname == request.Nickname);
        if (user is null) return NotFound("User not found");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Nickname)
            },
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds
        );

        var encoded = new JwtSecurityTokenHandler().WriteToken(token);
        return Ok(new { accessToken = encoded, userId = user.Id, nickname = user.Nickname });
    }
}
