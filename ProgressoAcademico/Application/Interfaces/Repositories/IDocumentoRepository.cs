using ProgressoAcademico.Models;

namespace ProgressoAcademico.Application.Interfaces.Repositories;

public interface IDocumentoRepository
{
    Task<IReadOnlyList<Documento>> ListarPorSolicitacaoAsync(int solicitacaoProgressaoId, int usuarioId);
    Task<Documento?> ObterPorIdEUsuarioAsync(int documentoId, int usuarioId, bool incluirArquivo);
    Task<bool> TipoDocumentoExisteAsync(int tipoDocumentoId);
    Task<TipoDocumento?> ObterTipoDocumentoComprovanteAtividadeAsync();
    Task<List<TipoDocumento>> ListarTiposDocumentoAsync();
    Task AdicionarAsync(Documento documento);
    void Remover(Documento documento);
    Task SalvarAlteracoesAsync();
}
