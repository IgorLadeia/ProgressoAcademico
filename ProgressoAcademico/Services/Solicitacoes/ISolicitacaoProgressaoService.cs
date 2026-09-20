using ProgressoAcademico.Application.DTOs.Solicitacoes;

namespace ProgressoAcademico.Services.Solicitacoes;

public interface ISolicitacaoProgressaoService
{
    Task<IReadOnlyList<SolicitacaoResumoDto>> ListarPorUsuarioAsync(int usuarioId);
    Task<SolicitacaoDetalheDto?> ObterDetalheAsync(int solicitacaoProgressaoId);
    Task<SolicitacaoDetalheDto?> ObterDetalheDoProfessorAsync(int usuarioId, int solicitacaoProgressaoId);
    Task<ProfessorDashboardDto> ObterDashboardProfessorAsync(int usuarioId);
    Task<CriarSolicitacaoResultadoDto> CriarAsync(int usuarioId, CriarSolicitacaoDto dto);
    Task<SubmeterSolicitacaoResultadoDto> SubmeterAsync(int usuarioId, int solicitacaoProgressaoId);
}
