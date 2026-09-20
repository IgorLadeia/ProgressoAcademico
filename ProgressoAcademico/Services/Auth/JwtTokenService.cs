using Microsoft.IdentityModel.Tokens;
using ProgressoAcademico.Models;
using ProgressoAcademico.Models.DTOs.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProgressoAcademico.Services.Auth;

public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public JwtTokenResult GerarToken(Usuario usuario)
    {
        var issuer = _configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException("Jwt:Issuer nao foi configurado.");
        var audience = _configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException("Jwt:Audience nao foi configurado.");
        var key = _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("Jwt:Key nao foi configurado.");

        var expirationMinutes = _configuration.GetValue("Jwt:ExpirationMinutes", 120);
        var expiraEmUtc = DateTime.UtcNow.AddMinutes(expirationMinutes);
        var nomeExibicao = string.IsNullOrWhiteSpace(usuario.UsuarioPerfil?.Apelido)
            ? usuario.Nome
            : usuario.UsuarioPerfil.Apelido.Trim();

        var claims = UsuarioClaimsFactory.CriarClaims(usuario, nomeExibicao);

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiraEmUtc,
            signingCredentials: credentials);

        return new JwtTokenResult
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiraEmUtc = expiraEmUtc
        };
    }
}
