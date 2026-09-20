using ProgressoAcademico.Application.DTOs.Documentos;
using ProgressoAcademico.Models.ViewModels.Documentos;

namespace ProgressoAcademico.Services.Documentos;

public interface IDocumentoService
{
    Task<IReadOnlyList<DocumentoResumoDto>> ListarPorSolicitacaoAsync(int usuarioId, int solicitacaoProgressaoId);
    Task PreencherOpcoesUploadAsync(DocumentoUploadViewModel model);
    Task<UploadDocumentoResultadoDto> EnviarAsync(int usuarioId, DocumentoUploadViewModel model);
    Task<DocumentoDownloadDto?> ObterDownloadAsync(int usuarioId, int documentoId);
    Task<bool> ExcluirAsync(int usuarioId, int documentoId);
}
