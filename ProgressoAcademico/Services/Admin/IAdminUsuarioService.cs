using ProgressoAcademico.Application.DTOs.Admin;

namespace ProgressoAcademico.Services.Admin;

public interface IAdminUsuarioService
{
    Task<IReadOnlyList<AdminUsuarioAcessoDto>> ListarAsync();
    Task<AdminDecisaoResultadoDto> AlterarPerfilAsync(int usuarioId, int adminId, string perfil);
    Task<AdminDecisaoResultadoDto> AlterarNivelAcessoAsync(int usuarioId, int adminId, string nivelAcesso);
    Task<AdminDecisaoResultadoDto> DefinirPodeRevisarAsync(int usuarioId, int adminId, bool podeRevisar);
    Task<AdminDecisaoResultadoDto> DefinirAtivoAsync(int usuarioId, int adminId, bool ativo);
}
