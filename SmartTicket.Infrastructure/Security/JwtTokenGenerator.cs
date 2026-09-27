using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SmartTicket.Application.Interface;
using SmartTicket.Domain.Entity;

namespace SmartTicket.Infrastructure.Security;

public class JwtTokenGenerator(IConfiguration configuration) : IJwtTokenGenerator
{
    private readonly IConfiguration _configuration = configuration;

    public (string Token, DateTime ExpiraEn) GenerateToken(Tbl_Usuario usuario, string rolNombre)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expiraEn = DateTime.UtcNow.AddHours(double.Parse(jwtSettings["ExpiraHoras"] ?? "8"));

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.UsuarioId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
            new Claim(ClaimTypes.Name, usuario.Nombre),
            new Claim(ClaimTypes.Role, rolNombre)
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: expiraEn,
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiraEn);
    }
}