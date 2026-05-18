using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API_AchadosEPerdidos.Features.Users.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace API_AchadosEPerdidos.Shared.Security;

public class TokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GerarToken(Usuario usuario)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var chaveSecret = _configuration["JwtSettings:Secret"] ?? throw new Exception("Chave JWT não configurada.");
        var key = Encoding.ASCII.GetBytes(chaveSecret);
        var expiraEmHoras = double.Parse(_configuration["JwtSettings:ExpireHours"] ?? "8");

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim("AcessoId", usuario.AcessoId.ToString())
            }),
            Expires = DateTime.UtcNow.AddHours(expiraEmHoras),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}