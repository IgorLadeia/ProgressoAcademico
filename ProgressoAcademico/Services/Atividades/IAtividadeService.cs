using ProgressoAcademico.Application.DTOs.Atividades;
using ProgressoAcademico.Models.ViewModels.Atividades;

namespace ProgressoAcademico.Services.Atividades;

public interface IAtividadeService
{
    Task<IReadOnlyList<AtividadeResumoDto>> ListarPorSolicitacaoAsync(int usuarioId, int solicitacaoProgressaoId);
    Task PreencherOpcoesAsync(AtividadeFormViewModel model);
    Task<CriarAtividadeResultadoDto> CriarAsync(int usuarioId, AtividadeFormViewModel model);
    Task<CriarAtividadeResultadoDto> AtualizarAsync(int usuarioId, AtividadeFormViewModel model);
    Task<bool> ExcluirAsync(int usuarioId, int atividadeId);
}
