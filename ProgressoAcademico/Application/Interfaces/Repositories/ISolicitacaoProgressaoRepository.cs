using ProgressoAcademico.Models;

namespace ProgressoAcademico.Application.Interfaces.Repositories;

public interface ISolicitacaoProgressaoRepository
{
    Task<IReadOnlyList<SolicitacaoProgressao>> ListarPorUsuarioAsync(int usuarioId);
    Task<SolicitacaoProgressao?> ObterDetalhePorIdAsync(int solicitacaoProgressaoId);
    Task<SolicitacaoProgressao?> ObterDetalhePorIdEUsuarioAsync(int solicitacaoProgressaoId, int usuarioId);
    Task<SolicitacaoProgressao?> ObterParaEdicaoPorIdEUsuarioAsync(int solicitacaoProgressaoId, int usuarioId);
    Task<SolicitacaoProgressao?> ObterParaSubmissaoPorIdEUsuarioAsync(int solicitacaoProgressaoId, int usuarioId);
    Task<bool> ExisteSolicitacaoAbertaAsync(int usuarioId);
    Task<StatusSolicitacao?> ObterStatusPorNomeAsync(string nome);
    Task<bool> TipoProgressoExisteAsync(int tipoProgressoId);
    Task<bool> NivelExisteAsync(int nivelId);
    Task AdicionarAsync(SolicitacaoProgressao solicitacao);
    Task SalvarAlteracoesAsync();
}
