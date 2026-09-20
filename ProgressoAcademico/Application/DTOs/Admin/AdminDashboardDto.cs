using ProgressoAcademico.Application.DTOs.Solicitacoes;

namespace ProgressoAcademico.Application.DTOs.Admin;

public class AdminDashboardDto
{
    public int TotalSolicitacoes { get; set; }
    public int AguardandoAtribuicao { get; set; }
    public int EmProcessamento { get; set; }
    public int EmRevisao { get; set; }
    public int AguardandoDecisaoFinal { get; set; }
    public int AjustesSolicitados { get; set; }
    public int Aprovadas { get; set; }
    public int Negadas { get; set; }
    public int Encerradas { get; set; }
    public int TotalProfessores { get; set; }
    public int TotalRevisores { get; set; }
    public int TotalDocumentos { get; set; }
    public int TotalAtividades { get; set; }
    public int PercentualMedioPreenchimento { get; set; }
    public IReadOnlyList<AdminSolicitacaoResumoDto> Recentes { get; set; } = Array.Empty<AdminSolicitacaoResumoDto>();
}
