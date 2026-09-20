using ProgressoAcademico.Application.DTOs.Admin;
using ProgressoAcademico.Application.DTOs.Documentos;
using ProgressoAcademico.Models.ViewModels.Admin;

namespace ProgressoAcademico.Services.Revisor;

public interface IRevisorSolicitacaoService
{
    Task<AdminDashboardDto> ObterDashboardAsync(int revisorUsuarioId);
    Task<IReadOnlyList<AdminSolicitacaoResumoDto>> ListarAsync(int revisorUsuarioId);
    Task<AdminSolicitacaoDetalheDto?> ObterDetalheAsync(int solicitacaoProgressaoId, int revisorUsuarioId);
    Task<AdminDecisaoResultadoDto> AvaliarAtividadeAsync(AdminAvaliarAtividadeViewModel model, int revisorUsuarioId);
    Task<AdminDecisaoResultadoDto> DevolverParaAjustesAsync(int solicitacaoProgressaoId, int revisorUsuarioId, string parecer);
    Task<AdminDecisaoResultadoDto> EncaminharParaAdmAsync(int solicitacaoProgressaoId, int revisorUsuarioId, string parecer);
    Task<DocumentoDownloadDto?> ObterDocumentoAsync(int documentoId, int revisorUsuarioId);
}
