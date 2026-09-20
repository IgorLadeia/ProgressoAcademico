using ProgressoAcademico.Models;
using System.Security.Claims;

namespace ProgressoAcademico.Services.Auth;

public static class UsuarioClaimsFactory
{
    public static List<Claim> CriarClaims(Usuario usuario, string? nomeExibicao = null)
    {
        var nome = string.IsNullOrWhiteSpace(nomeExibicao) ? usuario.Nome : nomeExibicao;
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuario.UsuarioId.ToString()),
            new(ClaimTypes.Name, nome),
            new(ClaimTypes.Email, usuario.Email),
            new(ClaimTypes.Role, usuario.PerfilAcesso)
        };

        if (usuario.PodeRevisar && usuario.PerfilAcesso != PerfisAcesso.Revisor)
            claims.Add(new Claim(ClaimTypes.Role, PerfisAcesso.Revisor));

        return claims;
    }
}
