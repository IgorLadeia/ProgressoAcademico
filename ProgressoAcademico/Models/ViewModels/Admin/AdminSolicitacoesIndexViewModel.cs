using ProgressoAcademico.Application.DTOs.Admin;

namespace ProgressoAcademico.Models.ViewModels.Admin;

public class AdminSolicitacoesIndexViewModel
{
    public AdminSolicitacaoFiltroViewModel Filtro { get; set; } = new();
    public IReadOnlyList<AdminSolicitacaoResumoDto> Solicitacoes { get; set; } = Array.Empty<AdminSolicitacaoResumoDto>();
}
