using ProgressoAcademico.Application.DTOs.Atividades;
using ProgressoAcademico.Application.DTOs.Documentos;

namespace ProgressoAcademico.Models.ViewModels.Admin;

public class AdminAtividadeRevisaoViewModel
{
    public int SolicitacaoProgressaoId { get; set; }
    public AtividadeResumoDto Atividade { get; set; } = new();
    public IReadOnlyList<DocumentoResumoDto> Documentos { get; set; } = Array.Empty<DocumentoResumoDto>();
    public bool ExibirPareceresRevisores { get; set; } = true;
    public bool EdicaoDoRevisor { get; set; }
    public bool DestacarParecerAtual { get; set; }
}
