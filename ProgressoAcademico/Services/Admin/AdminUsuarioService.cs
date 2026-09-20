using ProgressoAcademico.Application.DTOs.Admin;
using ProgressoAcademico.Application.Interfaces.Repositories;
using ProgressoAcademico.Models;

namespace ProgressoAcademico.Services.Admin;

public class AdminUsuarioService : IAdminUsuarioService
{
    private readonly IAdminUsuarioRepository _usuarioRepository;

    public AdminUsuarioService(IAdminUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public async Task<IReadOnlyList<AdminUsuarioAcessoDto>> ListarAsync()
    {
        var usuarios = await _usuarioRepository.ListarAsync();
        return usuarios.Select(u => new AdminUsuarioAcessoDto
        {
            UsuarioId = u.UsuarioId,
            Nome = u.Nome,
            Email = u.Email,
            PerfilAcesso = u.PerfilAcesso,
            PodeRevisar = u.PodeRevisar,
            Ativo = u.Ativo,
            DataCriacao = u.DataCriacao,
            DataExclusao = u.DataExclusao,
            AvatarFallbackUrl = FotoPerfilUrl(u) ?? AvatarFallbackUrl(u.UsuarioId)
        }).ToList();
    }

    private static string? FotoPerfilUrl(Usuario usuario)
    {
        var url = usuario.UsuarioPerfil?.FotoPerfil;
        return !string.IsNullOrWhiteSpace(url) && url.StartsWith("/images/")
            ? url
            : null;
    }

    private static string AvatarFallbackUrl(int usuarioId)
    {
        var indice = Math.Abs(usuarioId % 4) + 1;
        return $"/images/avatars/avatar-{indice}.svg";
    }

    public async Task<AdminDecisaoResultadoDto> AlterarPerfilAsync(int usuarioId, int adminId, string perfil)
    {
        if (!PerfisAcesso.EhValido(perfil))
            return AdminDecisaoResultadoDto.Falha("Perfil de acesso invalido.");

        if (usuarioId == adminId && perfil != PerfisAcesso.Administrador)
            return AdminDecisaoResultadoDto.Falha("O administrador nao pode remover o proprio acesso administrativo.");

        var usuario = await _usuarioRepository.ObterAsync(usuarioId);

        if (usuario == null)
            return AdminDecisaoResultadoDto.Falha("Usuario nao encontrado.");

        usuario.PerfilAcesso = perfil;
        if (perfil == PerfisAcesso.Administrador)
            usuario.PodeRevisar = false;
        await _usuarioRepository.SalvarAlteracoesAsync();
        return AdminDecisaoResultadoDto.Ok();
    }

    public async Task<AdminDecisaoResultadoDto> AlterarNivelAcessoAsync(int usuarioId, int adminId, string nivelAcesso)
    {
        var niveisValidos = new[] { PerfisAcesso.Professor, PerfisAcesso.Revisor, PerfisAcesso.Administrador };
        if (!niveisValidos.Contains(nivelAcesso))
            return AdminDecisaoResultadoDto.Falha("Nivel de acesso invalido.");

        if (usuarioId == adminId && nivelAcesso != PerfisAcesso.Administrador)
            return AdminDecisaoResultadoDto.Falha("O administrador nao pode reduzir o proprio nivel de acesso.");

        var usuario = await _usuarioRepository.ObterAsync(usuarioId);

        if (usuario == null)
            return AdminDecisaoResultadoDto.Falha("Usuario nao encontrado.");

        if (usuario.PerfilAcesso == PerfisAcesso.Administrador && usuarioId != adminId)
            return AdminDecisaoResultadoDto.Falha("Um administrador nao pode modificar o nivel de outro administrador.");

        usuario.PerfilAcesso = nivelAcesso == PerfisAcesso.Administrador
            ? PerfisAcesso.Administrador
            : PerfisAcesso.Professor;
        usuario.PodeRevisar = nivelAcesso == PerfisAcesso.Revisor;

        await _usuarioRepository.SalvarAlteracoesAsync();
        return AdminDecisaoResultadoDto.Ok();
    }

    public async Task<AdminDecisaoResultadoDto> DefinirPodeRevisarAsync(int usuarioId, int adminId, bool podeRevisar)
    {
        var usuario = await _usuarioRepository.ObterAsync(usuarioId);

        if (usuario == null)
            return AdminDecisaoResultadoDto.Falha("Usuario nao encontrado.");

        if (usuario.PerfilAcesso == PerfisAcesso.Administrador && podeRevisar)
            return AdminDecisaoResultadoDto.Falha("A funcao de revisao deve ser atribuida a professores.");

        usuario.PodeRevisar = podeRevisar;
        await _usuarioRepository.SalvarAlteracoesAsync();
        return AdminDecisaoResultadoDto.Ok();
    }

    public async Task<AdminDecisaoResultadoDto> DefinirAtivoAsync(int usuarioId, int adminId, bool ativo)
    {
        if (usuarioId == adminId && !ativo)
            return AdminDecisaoResultadoDto.Falha("O administrador nao pode inativar o proprio usuario.");

        var usuario = await _usuarioRepository.ObterAsync(usuarioId);

        if (usuario == null)
            return AdminDecisaoResultadoDto.Falha("Usuario nao encontrado.");

        usuario.Ativo = ativo;
        usuario.DataExclusao = ativo ? null : DateTime.UtcNow;

        await _usuarioRepository.SalvarAlteracoesAsync();
        return AdminDecisaoResultadoDto.Ok();
    }
}
