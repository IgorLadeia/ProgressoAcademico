using ProgressoAcademico.Application.DTOs.Solicitacoes;

namespace ProgressoAcademico.Models.ViewModels.ProfessorPortal;

public class ProfessorDashboardViewModel
{
    public string NomeProfessor { get; set; } = string.Empty;
    public int TotalSolicitacoes { get; set; }
    public int EmProcessamento { get; set; }
    public int AjustesSolicitados { get; set; }
    public int Aprovadas { get; set; }
    public int Negadas { get; set; }
    public int Encerradas { get; set; }
    public IReadOnlyList<SolicitacaoResumoDto> SolicitacoesRecentes { get; set; } = Array.Empty<SolicitacaoResumoDto>();
}
