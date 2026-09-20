using ProgressoAcademico.Application.Interfaces.Repositories;
using ProgressoAcademico.Models;
using ProgressoAcademico.Models.DTOs.Auth;
using ProgressoAcademico.Models.ViewModels.Login;

namespace ProgressoAcademico.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthService(IUsuarioRepository usuarioRepository, IJwtTokenService jwtTokenService)
    {
        _usuarioRepository = usuarioRepository;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<LoginResponseDto?> AutenticarAsync(LoginViewModel model)
    {
        var usuario = await _usuarioRepository.ObterPorEmailAsync(model.Email);

        if (usuario == null || !usuario.Ativo || !PerfisAcesso.EhValido(usuario.PerfilAcesso))
            return null;

        if (!SenhaValida(model.Senha, usuario.SenhaHash))
            return null;

        var token = _jwtTokenService.GerarToken(usuario);
        var nomeExibicao = string.IsNullOrWhiteSpace(usuario.UsuarioPerfil?.Apelido)
            ? usuario.Nome
            : usuario.UsuarioPerfil.Apelido.Trim();

        return new LoginResponseDto
        {
            Token = token.Token,
            ExpiraEmUtc = token.ExpiraEmUtc,
            Usuario = new UsuarioAutenticadoDto
            {
                UsuarioId = usuario.UsuarioId,
                Nome = nomeExibicao,
                Email = usuario.Email,
                PerfilAcesso = usuario.PerfilAcesso,
                PodeRevisar = usuario.PodeRevisar
            }
        };
    }

    private static bool SenhaValida(string senha, string senhaHash)
    {
        if (string.IsNullOrWhiteSpace(senhaHash))
            return false;

        try
        {
            return BCrypt.Net.BCrypt.Verify(senha, senhaHash);
        }
        catch
        {
            return false;
        }
    }
}
