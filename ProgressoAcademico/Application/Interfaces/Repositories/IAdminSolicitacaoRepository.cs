using ProgressoAcademico.Application.DTOs.Admin;
using ProgressoAcademico.Models;

namespace ProgressoAcademico.Application.Interfaces.Repositories;

public interface IAdminSolicitacaoRepository
{
    Task<IReadOnlyList<SolicitacaoProgressao>> ListarAsync(AdminSolicitacaoFiltroDto filtro);
    Task<SolicitacaoProgressao?> ObterDetalheAsync(int solicitacaoProgressaoId);
    Task<int> ContarProfessoresAsync();
    Task<int> ContarRevisoresAsync();
    Task<int> ContarDocumentosAsync();
    Task<int> ContarAtividadesAsync();
    Task<List<Usuario>> ListarRevisoresAsync();
    Task<Usuario?> ObterUsuarioAsync(int usuarioId);
    Task<List<Usuario>> ObterUsuariosAsync(IReadOnlyCollection<int> usuarioIds);
    Task<StatusSolicitacao?> ObterStatusPorNomeAsync(string nome);
    Task<List<StatusSolicitacao>> ListarStatusAsync();
    Task<List<TipoProgresso>> ListarTiposProgressoAsync();
    Task<List<Nivel>> ListarNiveisAsync();
    Task<Atividade?> ObterAtividadeAsync(int atividadeId, int solicitacaoProgressaoId);
    Task<Documento?> ObterDocumentoAsync(int documentoId);
    Task<SolicitacaoRevisor?> ObterSolicitacaoRevisorAsync(int solicitacaoProgressaoId, int revisorUsuarioId);
    Task AdicionarSolicitacaoRevisorAsync(SolicitacaoRevisor solicitacaoRevisor);
    Task SalvarAlteracoesAsync();
}
