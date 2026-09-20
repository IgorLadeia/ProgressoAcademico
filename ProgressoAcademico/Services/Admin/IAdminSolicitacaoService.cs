using ProgressoAcademico.Application.DTOs.Admin;
using ProgressoAcademico.Models.ViewModels.Admin;
using ProgressoAcademico.Application.DTOs.Documentos;

namespace ProgressoAcademico.Services.Admin;

public interface IAdminSolicitacaoService
{
    Task<AdminDashboardDto> ObterDashboardAsync();
    Task<IReadOnlyList<AdminSolicitacaoResumoDto>> ListarAsync(AdminSolicitacaoFiltroDto filtro);
    Task<AdminSolicitacaoDetalheDto?> ObterDetalheAsync(int solicitacaoProgressaoId);
    Task PreencherFiltrosAsync(AdminSolicitacaoFiltroViewModel filtro);
    Task<AdminCorrigirSolicitacaoViewModel?> ObterParaCorrecaoAsync(int solicitacaoProgressaoId);
    Task PreencherCorrecaoAsync(AdminCorrigirSolicitacaoViewModel model);
    Task<AdminAtribuirRevisorViewModel?> ObterParaAtribuicaoAsync(int solicitacaoProgressaoId);
    Task PreencherAtribuicaoAsync(AdminAtribuirRevisorViewModel model);
    Task<AdminDecisaoResultadoDto> AtribuirRevisorAsync(AdminAtribuirRevisorViewModel model, int adminId);
    Task<AdminDecisaoResultadoDto> CorrigirAsync(AdminCorrigirSolicitacaoViewModel model);
    Task<AdminDecisaoResultadoDto> AprovarAsync(int solicitacaoProgressaoId, int adminId, string parecer);
    Task<AdminDecisaoResultadoDto> ReprovarAsync(int solicitacaoProgressaoId, int adminId, string parecer);
    Task<AdminDecisaoResultadoDto> SolicitarAjustesAsync(int solicitacaoProgressaoId, int adminId, string parecer);
    Task<AdminDecisaoResultadoDto> EncerrarAsync(int solicitacaoProgressaoId, int adminId, string parecer);
    Task<AdminDecisaoResultadoDto> AvaliarAtividadeAsync(AdminAvaliarAtividadeViewModel model);
    Task<DocumentoDownloadDto?> ObterDocumentoAsync(int documentoId);
}
