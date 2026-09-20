using ProgressoAcademico.Models;

namespace ProgressoAcademico.Application.Interfaces.Repositories;

public interface IRevisorSolicitacaoRepository
{
    Task<IReadOnlyList<SolicitacaoProgressao>> ListarAtribuidasAsync(int revisorUsuarioId);
    Task<SolicitacaoProgressao?> ObterDetalheAtribuidaAsync(int solicitacaoProgressaoId, int revisorUsuarioId);
    Task<SolicitacaoRevisor?> ObterAtribuicaoAsync(int solicitacaoProgressaoId, int revisorUsuarioId);
    Task<StatusSolicitacao?> ObterStatusPorNomeAsync(string nome);
    Task<Atividade?> ObterAtividadeAsync(int atividadeId, int solicitacaoProgressaoId, int revisorUsuarioId);
    Task<AtividadeAvaliacaoRevisor?> ObterAvaliacaoAtividadeAsync(int atividadeId, int solicitacaoRevisorId);
    Task AdicionarAvaliacaoAtividadeAsync(AtividadeAvaliacaoRevisor avaliacao);
    Task<Documento?> ObterDocumentoAsync(int documentoId, int revisorUsuarioId);
    Task SalvarAlteracoesAsync();
}
