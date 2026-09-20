using ProgressoAcademico.Application.DTOs.Atividades;
using ProgressoAcademico.Application.DTOs.Documentos;
using ProgressoAcademico.Application.DTOs.Elegibilidade;

namespace ProgressoAcademico.Application.DTOs.Admin;

public class AdminSolicitacaoDetalheDto : AdminSolicitacaoResumoDto
{
    public string? Observacao { get; set; }
    public string? ParecerFinal { get; set; }
    public string? ParecerRevisor { get; set; }
    public DateTime? DataParecerRevisor { get; set; }
    public string? RevisadoPor { get; set; }
    public string? AtribuidoPor { get; set; }
    public DateTime? DataRevisao { get; set; }
    public IReadOnlyList<SolicitacaoRevisorDto> Revisores { get; set; } = Array.Empty<SolicitacaoRevisorDto>();
    public IReadOnlyList<AtividadeResumoDto> Atividades { get; set; } = Array.Empty<AtividadeResumoDto>();
    public IReadOnlyList<DocumentoResumoDto> Documentos { get; set; } = Array.Empty<DocumentoResumoDto>();
    public ElegibilidadeResultadoDto? Elegibilidade { get; set; }
}
