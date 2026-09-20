using ProgressoAcademico.Models;

namespace ProgressoAcademico.Application.Interfaces.Repositories;

public interface IAtividadeRepository
{
    Task<IReadOnlyList<Atividade>> ListarPorSolicitacaoAsync(int solicitacaoProgressaoId, int usuarioId);
    Task<List<SubTipoAtividade>> ListarSubtiposAsync();
    Task<SubTipoAtividade?> ObterSubtipoAsync(int subTipoAtividadeId);
    Task<Atividade?> ObterPorIdEUsuarioAsync(int atividadeId, int usuarioId);
    Task AdicionarAsync(Atividade atividade);
    void Remover(Atividade atividade);
    Task SalvarAlteracoesAsync();
}
