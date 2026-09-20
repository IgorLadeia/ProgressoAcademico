using ProgressoAcademico.Application.DTOs.Documentos;
using ProgressoAcademico.Application.DTOs.Solicitacoes;

namespace ProgressoAcademico.Models.ViewModels.Documentos;

public class DocumentosSolicitacaoViewModel
{
    public SolicitacaoDetalheDto Solicitacao { get; set; } = new();
    public DocumentoUploadViewModel Upload { get; set; } = new();
    public IReadOnlyList<DocumentoResumoDto> Documentos { get; set; } = Array.Empty<DocumentoResumoDto>();
}
