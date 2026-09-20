using ProgressoAcademico.Application.DTOs.Atividades;
using ProgressoAcademico.Application.DTOs.Documentos;
using ProgressoAcademico.Application.DTOs.Solicitacoes;

namespace ProgressoAcademico.Models.ViewModels.Atividades;

public class AtividadesSolicitacaoViewModel
{
    public SolicitacaoDetalheDto Solicitacao { get; set; } = new();
    public AtividadeFormViewModel Formulario { get; set; } = new();
    public IReadOnlyList<AtividadeResumoDto> Atividades { get; set; } = Array.Empty<AtividadeResumoDto>();
    public IReadOnlyList<DocumentoResumoDto> Documentos { get; set; } = Array.Empty<DocumentoResumoDto>();
    public string GrupoAtual { get; set; } = "Ensino";
}
